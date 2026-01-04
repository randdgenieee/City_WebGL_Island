public class UnemiShouldCloseEvent
{
	private object _sender;

	public object Sender => _sender;

	public UnemiShouldCloseEvent(object sender)
	{
		_sender = sender;
	}
}
