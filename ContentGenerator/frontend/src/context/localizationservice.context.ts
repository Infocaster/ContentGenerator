import { createContext } from "@lit/context";
import type { ILocalizationService } from "../util/umbraco/localization.service";
export type { ILocalizationService } from "../util/umbraco/localization.service";
export const localizationServiceKey = 'localizationService';
export const localizationServiceContext = createContext<ILocalizationService>(localizationServiceKey);