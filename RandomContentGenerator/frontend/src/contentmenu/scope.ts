import { createContext } from "@lit/context";

export interface IContentMenuScope extends angular.IScope {

    currentNode: IContentMenuScopeContent;
}

interface IContentMenuScopeContent {
    
    id: string;
}

export const contentMenuScopeKey = "contentMenuScope";
export const contentMenuScopeContext = createContext<IContentMenuScope>(contentMenuScopeKey);