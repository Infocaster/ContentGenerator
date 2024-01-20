/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
export interface ILocalizationService {
    localizeMany(keys: Array<string>): angular.IPromise<Array<string>>;
    localize(value: string, tokens?: Array<string>, fallbackValue?: string): angular.IPromise<string>;
    tokenReplace(value: string, tokens: Array<string>): string;
}