import { LitElement, html } from "lit";
import { AngularBridgeMixin } from "../util/bridge/angularbridge.mixin";
import { customElement } from "lit/decorators.js";
import './content.lit';
import '../util/bridge/angulariconregistry.lit'

export const contentGeneratorTag = 'cg-main';

@customElement(contentGeneratorTag)
export class ContentGenerator extends AngularBridgeMixin(LitElement, html`<cg-content></cg-content>`) {

}