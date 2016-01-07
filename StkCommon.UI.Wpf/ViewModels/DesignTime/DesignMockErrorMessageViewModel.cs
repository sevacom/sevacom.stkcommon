namespace StkCommon.UI.Wpf.ViewModels.DesignTime
{
	public class DesignMockErrorMessageViewModel : ErrorMessageViewModel
	{
		public DesignMockErrorMessageViewModel()
			: base(string.Empty, string.Empty, string.Empty)
		{
			Caption = "Удаление объекта";
			Message = @"Удаление объекта не выполнено.";
			Details = @"Удаление объекта не выполнено, так как неправильно указан идентификатор объекта.
ghdflghkldkfglhdkflhg
вапрдлвадрпвджалпрждварп
вапрдлварлпждвждарплджвалпжрд
вапрдлварлпжвджарплджважрпдл
варпдлвадрплждважплрдварпждлждв
вадпрлваджрплдвжарпллвждарп
лваждпрлваждплрвдларпдлвжапжрд
дварлпждвалдрлпжвдажрдпвлждарп
вжпрлждваплрвларпджвлажрплварп


вап
sd
gsd
fg
sdfg
s
вап
sdf
gs
вап
sd
fg
варпварпвапр";

			ShowDetails = true;
		}
	}
}
