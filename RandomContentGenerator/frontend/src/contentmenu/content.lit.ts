import { consume } from "@lit/context";
import { LitElement, css, html } from "lit";
import { customElement, state } from "lit/decorators.js";
import { httpContext } from "../context/http.context";
import { ensureExists } from "../util/ensure";
import { IContentMenuScope, contentMenuScopeContext } from "./scope";
import { IContentTypeService, contentTypeServiceContext } from "../context/contenttypeservice.context";

export const contentGeneratorTag = 'content-generator-content'

@customElement(contentGeneratorTag)
export class ContentGeneratorContent extends LitElement {

    @consume({ context: httpContext })
    private http?: angular.IHttpService;

    @consume({ context: contentMenuScopeContext })
    private scope?: IContentMenuScope;

    @consume({ context: contentTypeServiceContext })
    private contentTypeService?: IContentTypeService;

    @state()
    private loading: number = 0;

    @state()
    private contentTypes?: ISelectableContentType[];

    private async onClickGenerate(e: Event) {

        e.preventDefault();

        if (!(e instanceof SubmitEvent)) return;

        if (!(e.target instanceof HTMLFormElement)) return;

        let formData = new FormData(e.target);
        let mode = formData.get('mode')?.toString();
        let seed = formData.get('seed')?.toString();
        let amount = formData.get('amount')?.toString();
        let contentTypeIds = this.contentTypes?.filter(cts => cts.selected).map(cts => cts.id);

        this.loading++;
        try {

            ensureExists(this.http);
            ensureExists(this.scope);

            await this.http.post('/umbraco/backoffice/randomcontentgenerator/generator/generate', {
                contentId: this.scope.currentNode.id,
                seed: seed,
                amount: amount,
                optionalRatio: mode,
                contentTypes: contentTypeIds
            });
        }
        finally {

            this.loading--;
        }
    }

    private getButtonLoadingState(): 'waiting' | undefined {

        if (this.loading) return 'waiting';
    }

    async connectedCallback(): Promise<void> {
        
        super.connectedCallback();
        
        ensureExists(this.contentTypeService);
        ensureExists(this.scope);

        this.loading++;
        try {

            const response = await this.contentTypeService.getAllowedTypes(parseInt(this.scope.currentNode.id));
            this.contentTypes = response.map(el => {
                return {
                    id: el.id,
                    name: el.name,
                    icon: el.icon.split(' ', 1)[0],
                    alias: el.alias,
                    selected: false
                }
            });

            if (this.contentTypes.length === 1) {

                this.contentTypes[0].selected = true;
            }
        }
        finally {

            this.loading--;
        }
    }

    private renderContentTypeList = (): unknown => {

        if (!this.contentTypes?.length) return html`<i>This content does not support child pages</i>`;

        return this.contentTypes.map(this.renderNode)
    }

    private renderNode = (node: ISelectableContentType, index: number): unknown => {

        return html`
            <uui-ref-node-document-type .name=${node.name}
                                        .alias=${node.alias}
                                        .selected=${node.selected}
                                        selectable
                                        select-only="true"
                                        @selected=${() => {ensureExists(this.contentTypes); this.contentTypes[index].selected = true}}
                                        @deselected=${() => {ensureExists(this.contentTypes); this.contentTypes[index].selected = false}}>
                <uui-icon slot="icon" .name=${node.icon}></uui-icon>
            </uui-ref-node-document-type>
        `;
    }

    protected render(): unknown {
        
        return html`
            <uui-form>
                <form @submit=${this.onClickGenerate}>
                    <uui-form-layout-item>
                        <uui-label slot="label" for="contenttypes">Content types</uui-label>
                        <span slot="description">Which types of content should be generated?</span>
                        <uui-ref-list name="contenttypes">
                            ${this.renderContentTypeList()}
                        </uui-ref-list>
                    </uui-form-layout-item>
                    <uui-form-layout-item>
                        <uui-label slot="label" for="amount">Amount</uui-label>
                        <span slot="description">How many pages should be generated?</span>
                        <uui-input type="number" name="amount" label="Amount of pages" value="1" ></uui-input>
                    </uui-form-layout-item>
                    <uui-form-layout-item>
                        <uui-label slot="label" for="mode">Optional field population ratio</uui-label>
                        <span slot="description">This portion of optional fields will get populated. 1 means all, 0 means none.</span>
                        <uui-slider name="mode" label="Slider label" step="0.01" min="0" max="1" value="0.5"></uui-slider>
                    </uui-form-layout-item>
                    <uui-form-layout-item>
                        <uui-label slot="label" for="seed">Random seed</uui-label>
                        <span slot="description">(Optional) Enter a random seed here to influence random content generation</span>
                        <uui-input type="number" name="seed" label="Random seed" ></uui-input>
                    </uui-form-layout-item>
                    <uui-button type="submit"
                                label="Submit"
                                look="primary"
                                color="positive"
                                .state=${this.getButtonLoadingState()}>
                        Submit
                    </uui-button>
                </form>
            </uui-form>
        `;
    }
    
    public static styles = css`
    
        form {
            padding: 0 20px 20px;
        }
    `;
}

interface ISelectableContentType
{
    id: number;
    alias: string;
    name: string;
    icon: string;
    selected: boolean;
}