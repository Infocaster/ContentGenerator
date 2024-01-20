/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
import { LitElement, html } from "lit";
import { AngularBridgeMixin } from "../util/bridge/angularbridge.mixin";
import { customElement } from "lit/decorators.js";
import './content.lit';
import '../util/bridge/angulariconregistry.lit'

export const contentGeneratorTag = 'cg-main';

@customElement(contentGeneratorTag)
export class ContentGenerator extends AngularBridgeMixin(LitElement, html`<cg-content></cg-content>`) {

}