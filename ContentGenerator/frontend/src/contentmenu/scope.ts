/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
import { createContext } from "@lit/context";

export interface IContentMenuScope extends angular.IScope {

    currentNode: IContentMenuScopeContent;
}

interface IContentMenuScopeContent {
    
    id: string;
}

export const contentMenuScopeKey = "contentMenuScope";
export const contentMenuScopeContext = createContext<IContentMenuScope>(contentMenuScopeKey);