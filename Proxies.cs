using KokoLib.Nets;

namespace KokoLib;

public partial class Net
{
	public static IText Text => Net<IText>.proxy;


}
