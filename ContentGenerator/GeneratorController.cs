﻿using Microsoft.AspNetCore.Mvc;
using ContentGenerator.Request;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

namespace ContentGenerator
{
    [PluginController(Defaults.PluginArea)]
    public class GeneratorController : UmbracoAuthorizedApiController
    {
        private readonly IGenerateContentRequestHandler generateContentRequestHandler;

        public GeneratorController(IGenerateContentRequestHandler generateContentRequestHandler)
        {
            this.generateContentRequestHandler = generateContentRequestHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Generate(GenerateContentRequest request)
        {
            var context = new GenerateContentContext(Convert.ToInt32(User.Identity!.GetUserId()));

            await generateContentRequestHandler.HandleAsync(request, context);
            return Ok();
        }
    }
}
