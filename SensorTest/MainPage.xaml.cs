using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Sensors; // namespace que dá acesso aos sensores do aparelho
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.Phone.Devices.Notification; // namespace para poder invocar o vibrador

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace SensorTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // definir o acelerómetro
        public Accelerometer accelerometer { get; set; }
    
        // definir intervalos entre leituras do acelerómetro
        private uint desiredReportInterval { get; set; }

        // declarar um relógio
        private DispatcherTimer _dispatcherTimer;

        // limite a partir do qual o ficheiro é executado, ou seja,
        // quão sensível o telefone é ao movimento
        double limitThreshold = 1.9;

        // definir o vibrador
        VibrationDevice vibrador { get; set; }

        // tempo que o vibrador fica a funcionar, entre 0 e 5 segundos
        // qualquer outro valor dá erro
        // variável em milisegundos
        double limitVibrate = 300.0;


        public MainPage()
        {
            this.InitializeComponent();

            // inicializar o acelerómetro
            accelerometer = Accelerometer.GetDefault();

            // inicializar o vibrador
            vibrador = VibrationDevice.GetDefault();
            

            // verificar se há um acelerómetro e, se estiver, criar um evento de captura
            if (accelerometer != null)
            {
                // seleccionar um intervalo de resposta adequado tanto para a app
                // como para o próprio sensor
                uint minReportInterval = accelerometer.MinimumReportInterval;
                desiredReportInterval = minReportInterval > 16 ? minReportInterval : 16;

                // inicializer o relógio
                _dispatcherTimer = new DispatcherTimer();

                // cada vez que houver um tick, invocar o método para tocar ficheiro
                _dispatcherTimer.Tick += PlayMediaFile;

                // declarar a frequência com que ocorre o tick
                _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int) desiredReportInterval);

                // sempre que houver uma alteração no acelerómetro, invocar o método ReadingChanged
                //accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }
            else
            {
                MessageDialog ms = new MessageDialog("Não foi encontrado um acelerómetro");
                ms.ShowAsync();
            }

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void PlayMediaFile(object sender, object args)
        {
            AccelerometerReading reading = accelerometer.GetCurrentReading();
            if (reading != null)
            {
                // se o X exceder o limite tocar o ficheiro
                // o reading.Acceleration também existe para Y e Z, se fôr mais adequado
                if (reading.AccelerationX > limitThreshold)
                {
                    mediaFile.Play();
                    // vibrar quando toca, mas só se limitVibrate estiver entre 0 e 5
                    if ((limitVibrate < 6) || (limitVibrate > 0))
                    {
                        vibrador.Vibrate(TimeSpan.FromMilliseconds(limitThreshold);
                    }
                }
            }
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
        }

        // carregar o ficheiro multimédia quando o elemento fôr carregado
        private void mediaFile_Loaded(object sender, RoutedEventArgs e)
        {
            mediaFile.Source = new Uri("ms-appx:///Assets/SuperMarioBrothers-Coin.mp3");
        }

        // botão para iniciar o relógio, só toca o ficheiro se o relógio estiver a andar
        private void BTNStart_Click(object sender, RoutedEventArgs e)
        {
            if (!_dispatcherTimer.IsEnabled)
            {
                _dispatcherTimer.Start();
                BTNStart.Content = "Stop";
            }
            else
            {
                _dispatcherTimer.Stop();
                BTNStart.Content = "Start";
            }

        }
    }
}
