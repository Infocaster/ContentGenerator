import { LitElement, html } from "lit";
import { AngularBridgeMixin } from "../util/bridge/angularbridge.mixin";
import { customElement } from "lit/decorators.js";
import './content.lit';
import '../util/bridge/angulariconregistry.lit'

export const contentGeneratorTag = 'content-generator';

@customElement(contentGeneratorTag)
export class ContentGenerator extends AngularBridgeMixin(LitElement, html`<content-generator-content></content-generator-content>`) {

}