using ModularNet;
using ModularNet.Core;
using ModularNet.Sample.Modules;

namespace ModularNet.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ModularNet 방식으로 앱 생성
            var app = ModularAppFactory.CreateApp<AppModule>(args);

            // 개발 환경 설정
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
