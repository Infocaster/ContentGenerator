/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
import { ContentGenerator, contentGeneratorTag } from "./main.lit";
import { ILocalizationService, localizationServiceContext, localizationServiceKey } from "../context/localizationservice.context";
import { IIconHelper, iconHelperContext, iconHelperKey } from "../context/iconhelper.context";
import { httpContext, httpKey } from "../context/http.context";
import { contentMenuScopeContext, contentMenuScopeKey } from "./scope";
import { IContentTypeService, contentTypeServiceContext, contentTypeServiceKey } from "../context/contenttypeservice.context";

ngContentGeneratorMenu.alias = "ngContentGeneratorMenu";
ngContentGeneratorMenu.$inject = ["localizationService", "iconHelper", "$http", "contentTypeResource"]
export function ngContentGeneratorMenu(localizationService: ILocalizationService, iconHelper: IIconHelper, $http: angular.IHttpService, contentTypeResource: IContentTypeService): angular.IDirective {

    return {
        restrict: 'E',
        link: function (_scope, element) {

            let dashboardElement = document.createElement(contentGeneratorTag) as ContentGenerator;
            
            dashboardElement.SetContext(localizationService, localizationServiceContext, localizationServiceKey);
            dashboardElement.SetContext(iconHelper, iconHelperContext, iconHelperKey);
            dashboardElement.SetContext($http, httpContext, httpKey);
            dashboardElement.SetContext(contentTypeResource, contentTypeServiceContext, contentTypeServiceKey);
            dashboardElement.SetContext(_scope, contentMenuScopeContext, contentMenuScopeKey);
            
            element[0].appendChild(dashboardElement);
        }
    };
}