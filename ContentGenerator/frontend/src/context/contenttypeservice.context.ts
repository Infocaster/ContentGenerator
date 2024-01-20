/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
import { createContext } from "@lit/context";
import type { IContentTypeService } from "../util/umbraco/contenttype.service";
export type { IContentTypeService } from "../util/umbraco/contenttype.service";
export const contentTypeServiceKey = 'contentTypeService';
export const contentTypeServiceContext = createContext<IContentTypeService>(contentTypeServiceKey);