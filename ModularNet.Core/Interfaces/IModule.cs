using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularNet.Core.Interfaces;

public interface IModule
{
    void ConfigureServices(IServiceCollection services);
    void ConfigureApp(IApplicationBuilder app);
}
