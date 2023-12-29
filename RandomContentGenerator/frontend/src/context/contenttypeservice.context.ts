import { createContext } from "@lit/context";
import type { IContentTypeService } from "../util/umbraco/contenttype.service";
export type { IContentTypeService } from "../util/umbraco/contenttype.service";
export const contentTypeServiceKey = 'contentTypeService';
export const contentTypeServiceContext = createContext<IContentTypeService>(contentTypeServiceKey);