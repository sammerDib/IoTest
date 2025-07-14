using AutoMapper;

using UnitySC.Shared.Tools;

namespace ADCConfiguration.Services
{
    public class MapperService
    {
        private IMapper _mapper;
        public IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    var configuration = new MapperConfiguration(cfg =>
                    {
#warning ** USP ** MIGRATTION  check viability of following commented line "cfg.ConstructServicesUsing((t)..."
                        //cfg.ConstructServicesUsing((t) => { return ClassLocator.Default.GetInstance(t, Guid.NewGuid().ToString("N")); });
                        cfg.ConstructServicesUsing((t) => { return ClassLocator.Default.GetInstance(t); });
                        cfg.CreateMap<UnitySC.DataAccess.Dto.User, ViewModel.UserViewModel>().ConstructUsingServiceLocator().ReverseMap();
                        cfg.CreateMap<ViewModel.UserViewModel, ViewModel.UserViewModel>().ConstructUsingServiceLocator();
                    });
                    _mapper = configuration.CreateMapper();
                }

                return _mapper;
            }
        }
    }
}
