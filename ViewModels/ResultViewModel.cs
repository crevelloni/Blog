namespace Blog.ViewModels
{
    public class ResultViewModel<T>
    {

        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public ResultViewModel(string error)
        {
            Errors.Add(error);
        }

        public ResultViewModel(T data, List<string> errors)
        {
            Data = data;
            Errors = errors;
        }

        public ResultViewModel(List<string> errors)
        {
            Errors = errors;
        }

        public ResultViewModel(T data)
        {
            Data = data;
        }
    }
}
