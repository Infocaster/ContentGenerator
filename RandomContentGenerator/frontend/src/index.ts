import { ngContentGeneratorMenu } from "./contentmenu/directive";
import '@umbraco-ui/uui';

const module = angular.module('umbraco');

module.directive(ngContentGeneratorMenu.alias, ngContentGeneratorMenu);