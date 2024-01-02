import { ContextProvider, createContext } from "@lit/context";
import { LitElementConstructor } from "../litelementconstructor";
import { html } from "lit";

export function AngularBridgeMixin<TBase extends LitElementConstructor>(Base: TBase, contentTemplate: unknown){
    return class MainContext extends Base {

        _contextCollection: Record<string, unknown> = {};
        public SetContext<T>(service: T, context: ReturnType<typeof createContext<T>>, key: string){
            
            if (this._contextCollection[key]){
                (this._contextCollection[key] as ContextProvider<ReturnType<typeof createContext<T>>>).setValue(service);
            }
            else{
                this._contextCollection[key] = new ContextProvider(this, {context: context, initialValue: service});
            }
        }

        protected render(): unknown {
            return html`
                <cg-angular-icon-registry>
                    ${contentTemplate}
                </cg-angular-icon-registry>
            `;
        }
    }
}