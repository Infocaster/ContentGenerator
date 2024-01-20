/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
import { createContext } from "@lit/context";
import type { ILocalizationService } from "../util/umbraco/localization.service";
export type { ILocalizationService } from "../util/umbraco/localization.service";
export const localizationServiceKey = 'localizationService';
export const localizationServiceContext = createContext<ILocalizationService>(localizationServiceKey);