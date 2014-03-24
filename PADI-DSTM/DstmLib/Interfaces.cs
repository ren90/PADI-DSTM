namespace DSTMLib
{
    public interface MasterInterface
    {
        PADInt CreatePADInt(int uid);
        PADInt AccessPADInt(int uid);
    }

	public interface ServerInterface
	{
		int Read();
		void Write(int value);
	}
}
