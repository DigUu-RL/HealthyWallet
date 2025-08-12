using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace HealthyWallet.Infrastructure.CrossCutting.Conventions;

public class RoutePrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _route;

    public RoutePrefixConvention(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix)) throw new ArgumentNullException(nameof(prefix));
        _route = new AttributeRouteModel(new RouteAttribute(prefix));
    }
    
    public void Apply(ApplicationModel application)
    {
        foreach (ControllerModel controller in application.Controllers)
        {
            foreach (SelectorModel selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel is not null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_route, selector.AttributeRouteModel);
                    continue;
                }
                
                selector.AttributeRouteModel = _route;
            }
        }
    }
}