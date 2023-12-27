﻿using Microsoft.AspNetCore.Mvc;
using RandomContentGenerator.Request;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

namespace RandomContentGenerator
{
    [PluginController(Defaults.PluginArea)]
    public class GeneratorController(IGenerateContentRequestHandler generateContentRequestHandler) : UmbracoAuthorizedApiController
    {
        [HttpPost]
        public async Task<IActionResult> Generate(GenerateContentRequest request)
        {
            var context = new GenerateContentContext(Convert.ToInt32(User.Identity!.GetUserId()));

            await generateContentRequestHandler.HandleAsync(request, context);
            return Ok();
        }
    }
}
