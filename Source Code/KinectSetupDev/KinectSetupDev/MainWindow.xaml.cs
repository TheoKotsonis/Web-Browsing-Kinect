using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Diagnostics;

namespace KinectSetupDev
{

    public partial class MainWindow : Window
    {

        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll",
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]

        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEVENTF_WHEEL = 0x0800;

        const UInt32 WM_CLOSE = 0x0010;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;

        int screenHeight = (int)SystemParameters.VirtualScreenHeight + 100;
        int screenWidth = (int)SystemParameters.VirtualScreenWidth + 100;
        int screenYpos;
        int screenXpos;

        KinectSensor audsensor;
        private SpeechRecognitionEngine spRecognizer;
        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];

        private Stopwatch stopwatch = new Stopwatch();

        bool cbutton = false;
        bool rbutton = false;
        bool backbutton = false;
        bool forwbutton = false;
        bool newtabutton = false;
        bool nexttabutton = false;
        bool prevtabutton = false;
        bool zoominbutton = false;
        bool zoomoutbutton = false;
        bool GrabItem = false;
        bool Onsearch = false;
        public static bool keybo = false;
        public static bool urlbo = false;
        public static bool searchbo = false;
        
        int drdrop = 0;
        bool counterclicks = true;

        Dictionary<string, string> VoiceCommands = new Dictionary<string, string>();
        List<string> VSCmd = new List<string>();

//------------------------------------------- PROGRAM START --------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }

        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor oldSensor = (KinectSensor)e.OldValue;

            StopKinect(oldSensor);

            KinectSensor newSensor = (KinectSensor)e.NewValue;

            audsensor = newSensor;

            if (newSensor == null)
            {
                return;
            }

            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.1f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.5f
            };
            
            newSensor.SkeletonStream.Enable(parameters);

            AllVoiceCommands();

            newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(newSensor_AllFramesReady);
            newSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            spRecognizer = CreateSpRecognizer();

            try
            {
                newSensor.Start();
                newSensor.ElevationAngle = 14;
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
            }
            audsensor.AudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            spRecognizer.SetInputToAudioStream(
                audsensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));    
            spRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            audsensor.AudioSource.EchoCancellationMode = EchoCancellationMode.None;
            audsensor.AudioSource.AutomaticGainControlEnabled = false;
        }

        void newSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (closing)
            {
                return;
            }

            Skeleton first = GetFirstSkeleton(e);

            if (first == null)
            {
                return;
            }

            ProcessGesture(first.Joints[JointType.Head], first.Joints[JointType.HandLeft], first.Joints[JointType.HandRight], first.Joints[JointType.ShoulderRight], first.Joints[JointType.ShoulderLeft]);

        }

        private void ProcessGesture(Joint head, Joint handleft, Joint handright, Joint shoulderight, Joint shoulderleft)
        {
            Joint scJointLeft = handleft.ScaleTo(screenWidth, screenHeight, .3f, .3f);
            Joint scJointRight = handright.ScaleTo(screenWidth, screenHeight, .3f, .3f);

            if (handleft.Position.Z < head.Position.Z - 0.3f)
            {
                screenXpos = Convert.ToInt32(scJointLeft.Position.X);
                screenYpos = Convert.ToInt32(scJointLeft.Position.Y);

                if (!GrabItem)
                {
                    if (handright.Position.Y > shoulderight.Position.Y)
                    {

                        if ((handright.Position.Z < shoulderight.Position.Z - 0.4f) && (handright.Position.X > head.Position.X) && (handright.Position.Y < head.Position.Y + 0.1f) && cbutton)
                        {

                            cbutton = false;
                            stopwatch.Stop();
                            if ((!counterclicks) && (stopwatch.Elapsed.TotalSeconds < 1) && (stopwatch.Elapsed.TotalSeconds > 0))
                            {
                                counterclicks = true;
                                mouse_event(MOUSEEVENTF_LEFTDOWN, screenXpos, screenYpos, 0, 0);
                                Thread.Sleep(10);
                                mouse_event(MOUSEEVENTF_LEFTUP, screenXpos, screenYpos, 0, 0);
                                Thread.Sleep(100);
                                mouse_event(MOUSEEVENTF_LEFTDOWN, screenXpos, screenYpos, 0, 0);
                                Thread.Sleep(10);
                                mouse_event(MOUSEEVENTF_LEFTUP, screenXpos, screenYpos, 0, 0);
                                Thread.Sleep(10);
                            }
                            else
                            {
                                counterclicks = false;
                                mouse_event(MOUSEEVENTF_LEFTDOWN, screenXpos, screenYpos, 0, 0);
                                Thread.Sleep(10);
                                mouse_event(MOUSEEVENTF_LEFTUP, screenXpos, screenYpos, 0, 0);
                                Thread.Sleep(10);
                            }
                            stopwatch.Restart();
                        }
                        if ((handright.Position.X > head.Position.X) && (handright.Position.Z > shoulderight.Position.Z) && (handright.Position.Y < head.Position.Y) && (handright.Position.Y > shoulderight.Position.Y) && (drdrop == 1))
                        {
                            drdrop = 2;
                            mouse_event(MOUSEEVENTF_LEFTDOWN, screenXpos, screenYpos, 0, 0);
                            Thread.Sleep(10);
                        }
                        if ((handright.Position.X > head.Position.X) && (handright.Position.Z > shoulderleft.Position.Z) && (handright.Position.Y > head.Position.Y + 0.25f) && rbutton)
                        {
                            rbutton = false;
                            mouse_event(MOUSEVENTF_RIGHTDOWN, screenXpos, screenYpos, 0, 0);
                            Thread.Sleep(10);
                            mouse_event(MOUSEVENTF_RIGHTUP, screenXpos, screenYpos, 0, 0);
                            Thread.Sleep(10);
                        }
                        if ((handright.Position.Z > shoulderight.Position.Z - 0.3f) && (handright.Position.Z < shoulderight.Position.Z) && (handright.Position.X > head.Position.X) && (handright.Position.Y < head.Position.Y + 0.15f))
                        {
                            cbutton = true;
                            rbutton = true;
                            newtabutton = true;
                            if (drdrop == 2)
                            {
                                drdrop = 1;
                                mouse_event(MOUSEEVENTF_LEFTUP, screenXpos, screenYpos, 0, 0);
                                Thread.Sleep(10);
                            }
                            else drdrop = 1;
                        }
                    }
                    else
                    {
                        stopwatch.Stop();
                        stopwatch.Reset();
                    }
                    if ((handright.Position.Z < head.Position.Z - 0.5f) && (handleft.Position.Z < head.Position.Z - 0.5f) && (handright.Position.X < head.Position.X) && (handleft.Position.X < head.Position.X))
                    {
                        double Xs = Math.Pow(handright.Position.X - handleft.Position.X, 2);
                        double Ys = Math.Pow(handright.Position.Y - handleft.Position.Y, 2);
                        double currentHandDistance = Math.Pow(Xs + Ys, 0.5);

                        if ((currentHandDistance < 0.2f) && (currentHandDistance > 0.1f))
                        {
                            zoominbutton = true;
                            zoomoutbutton = true;
                        }
                        else if (currentHandDistance > 0.25f && zoominbutton)
                        {
                            zoominbutton = false;
                            SendKeys.SendWait("^({ADD})");
                            Thread.Sleep(1000);
                        }
                        else if (currentHandDistance < 0.05f && zoomoutbutton)
                        {
                            zoomoutbutton = false;
                            SendKeys.SendWait("^({SUBTRACT})");
                            Thread.Sleep(1000);
                        }

                        if (handright.Position.Y < 0.10 && handright.Position.Y > -0.10 && handleft.Position.Y < 0.10 && handleft.Position.Y > -0.10)
                        {
                            //Do nothing
                        }
                        if ((handright.Position.Y > 0) && (handleft.Position.Y > 0))
                        {
                            mouse_event(MOUSEVENTF_WHEEL, screenXpos, screenYpos, 30, 0);
                            Thread.Sleep(10);
                        }
                        else if (handright.Position.Y < 0 && handleft.Position.Y < 0)
                        {
                            mouse_event(MOUSEVENTF_WHEEL, screenXpos, screenYpos, -30, 0);
                            Thread.Sleep(10);
                        }
                    }
                }
                SetCursorPos(screenXpos, screenYpos);
            }
            else
            {
                stopwatch.Stop();
                stopwatch.Reset();
                if ((handleft.Position.Z > shoulderleft.Position.Z) && (handleft.Position.X > shoulderleft.Position.X) && (handleft.Position.X < head.Position.X) && (handleft.Position.Y > shoulderleft.Position.Y) && (handleft.Position.Y < head.Position.Y) && newtabutton)
                {
                    newtabutton = false;
                    SendKeys.SendWait("^({t})");
                    Thread.Sleep(10);
                }
                if ((handright.Position.Z < shoulderleft.Position.Z - 0.4f) && (handright.Position.X < head.Position.X - 0.04f) && (handright.Position.Y < head.Position.Y) && (handright.Position.Y > shoulderleft.Position.Y) && forwbutton)
                {
                    forwbutton = false;
                    SendKeys.SendWait("%({RIGHT})");
                    Thread.Sleep(10);
                }
                if ((handright.Position.Z < shoulderleft.Position.Z) && (handright.Position.X < shoulderleft.Position.X) && (handright.Position.Y < head.Position.Y) && (handright.Position.Y > shoulderleft.Position.Y) && backbutton)
                {
                    backbutton = false;
                    SendKeys.SendWait("%({LEFT})");
                    Thread.Sleep(10);
                }
                if ((handright.Position.Z < shoulderight.Position.Z - 0.5f) && (handright.Position.X > head.Position.X + 0.04f) && (handright.Position.Y < head.Position.Y) && (handright.Position.Y > shoulderight.Position.Y) && nexttabutton)
                {
                    nexttabutton = false;
                    SendKeys.SendWait("^({TAB})");
                    Thread.Sleep(10);
                }
                if ((handright.Position.X > head.Position.X + 0.04f) && (handright.Position.Z > shoulderight.Position.Z) && (handright.Position.Y < head.Position.Y) && (handright.Position.Y > shoulderight.Position.Y) && prevtabutton)
                {
                    prevtabutton = false;
                    SendKeys.SendWait("^+({TAB})");
                    Thread.Sleep(10);
                }
                if ((handright.Position.Z > shoulderight.Position.Z - 0.3f) && (handright.Position.Z < shoulderight.Position.Z))
                {
                    backbutton = true;
                    nexttabutton = true;
                    prevtabutton = true;
                    forwbutton = true;
                }
            }
        }

        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }

                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;

            }
        }

        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }

                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closing = true;
            StopKinect(kinectSensorChooser1.Kinect);
        }

        private SpeechRecognitionEngine CreateSpRecognizer()
        {

            RecognizerInfo ri = GetKinectRecognizer();

            SpeechRecognitionEngine speechrec;
            speechrec = new SpeechRecognitionEngine(ri.Id);

            Choices grammar = new Choices();

            grammar.Add("search on");
            grammar.Add("search off");
            VSCmd.Add("up");
            VSCmd.Add("down");
            VSCmd.Add("close");
            VSCmd.Add("maximize");
            VSCmd.Add("minimize");
            VSCmd.Add("restore");
            VSCmd.Add("keyboard");
            VSCmd.Add("click");
            VSCmd.Add("double click");
            VSCmd.Add("drag");
            VSCmd.Add("drop");
            VSCmd.Add("right click");
            VSCmd.Add("internet");
            VSCmd.Add("url");
            VSCmd.Add("forward");
            VSCmd.Add("back");
            VSCmd.Add("new tab");
            VSCmd.Add("close tab");
            VSCmd.Add("next tab");
            VSCmd.Add("previous tab");
            VSCmd.Add("reload");
            VSCmd.Add("homepage");
            VSCmd.Add("bookmark");
            VSCmd.Add("history");
            VSCmd.Add("download history");
            VSCmd.Add("print page");
            VSCmd.Add("save page");
            VSCmd.Add("source code");
            VSCmd.Add("developer tools");

            foreach (KeyValuePair<string, string> pair in VoiceCommands)
            {
                try
                {
                    grammar.Add(pair.Key);
                }
                catch (ArgumentException)
                {
                    //Do nothing
                }
            }

            foreach (string dkey in VSCmd)
            {
                try
                {
                    grammar.Add(dkey);
                }
                catch (ArgumentException)
                {
                    //Do nothing
                }
            }

            GrammarBuilder gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(grammar);

            Grammar g = new Grammar(gb);
            speechrec.LoadGrammar(g);

            speechrec.SpeechRecognized += RecSpeechRecognized;
            speechrec.SpeechHypothesized += RecSpeechHypothesized;
            speechrec.SpeechRecognitionRejected += RecSpeechRecognitionRejected;
            return speechrec;
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        private void RejectSpeech(RecognitionResult result, float conf)
        {
            Console.WriteLine(result.Text + " - " + conf);
        }

        private void RecSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            RejectSpeech(e.Result, (float)199.0);
        }

        private void RecSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {

        }

        private void RecSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < .75f)
            {
                RejectSpeech(e.Result, e.Result.Confidence);
                return;
            }
            IntPtr state = GetForegroundWindow();

            VoiceCommands.Clear();
            AllVoiceCommands();

            String rec;
            Console.WriteLine("Accept: {0} with confidence : {1}", e.Result.Text.ToUpperInvariant(), e.Result.Confidence);

            if (e.Result.Text.ToUpperInvariant() == "SEARCH ON")
            {
                System.Windows.Forms.MessageBox.Show(new Form() { TopMost = true }, "Αναζήτηση mode: ON");
                Onsearch = true;
            }
            if (e.Result.Text.ToUpperInvariant() == "SEARCH OFF")
            {
                System.Windows.Forms.MessageBox.Show(new Form() { TopMost = true }, "Αναζήτηση mode: OFF");
                Onsearch = false;
            }

            if (!Onsearch)
            {
                switch (rec = e.Result.Text.ToUpperInvariant())
                {
                    case "UP":
                        audsensor.ElevationAngle += 5;
                        break;
                    case "DOWN":
                        audsensor.ElevationAngle -= 5;
                        break;
                    case "CLOSE":
                        SendMessage(state, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        break;
                    case "MAXIMIZE":
                        ShowWindow(state, SW_MAXIMIZE);
                        break;
                    case "MINIMIZE":
                        ShowWindow(state, SW_MINIMIZE);
                        break;
                    case "RESTORE":
                        ShowWindow(state, SW_RESTORE);
                        break;
                    case "KEYBOARD":
                        if (!keybo)
                        {
                            keybo = true;
                            Keyboard kbo = new Keyboard();
                            kbo.Show();
                            SetForegroundWindow(kbo.Handle);
                        }
                        break;
                    case "CLICK":
                        mouse_event(MOUSEEVENTF_LEFTDOWN, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(10);
                        mouse_event(MOUSEEVENTF_LEFTUP, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(10);
                        break;
                    case "DOUBLE CLICK":
                        mouse_event(MOUSEEVENTF_LEFTDOWN, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(10);
                        mouse_event(MOUSEEVENTF_LEFTUP, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(10);
                        mouse_event(MOUSEEVENTF_LEFTUP, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(10);
                        break;
                    case "DRAG":
                        GrabItem = true;
                        mouse_event(MOUSEEVENTF_LEFTDOWN, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(10);
                        break;
                    case "DROP":
                        GrabItem = false;
                        mouse_event(MOUSEEVENTF_LEFTUP, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(100);
                        break;
                    case "RIGHT CLICK":
                        mouse_event(MOUSEVENTF_RIGHTDOWN, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(10);
                        mouse_event(MOUSEVENTF_RIGHTUP, screenXpos, screenYpos, 0, 0);
                        Thread.Sleep(10);
                        break;
                    case "INTERNET":
                        Process process = new Process();
                        process.StartInfo.FileName = "chrome";
                        process.StartInfo.Arguments = " --new-window";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        process.Start();
                        break;
                    case "URL":
                        SendKeys.SendWait("^({l})");
                        break;
                    case "FORWARD":
                        SendKeys.SendWait("%({RIGHT})");
                        break;
                    case "BACK":
                        SendKeys.SendWait("%({LEFT})");
                        break;
                    case "NEW TAB":
                        SendKeys.SendWait("^({t})");
                        break;
                    case "CLOSE TAB":
                        SendKeys.SendWait("^({w})");
                        break;
                    case "NEXT TAB":
                        SendKeys.SendWait("^({TAB})");
                        break;
                    case "PREVIOUS TAB":
                        SendKeys.SendWait("^(+({TAB}))");
                        break;
                    case "RELOAD":
                        SendKeys.SendWait("{F5}");
                        break;
                    case "HOMEPAGE":
                        SendKeys.SendWait("%({HOME})");
                        break;
                    case "BOOKMARK":
                        SendKeys.SendWait("^({d})");
                        break;
                    case "HISTORY":
                        SendKeys.SendWait("^({h})");
                        break;
                    case "DOWNLOAD HISTORY":
                        SendKeys.SendWait("^({j})");
                        break;
                    case "PRINT PAGE":
                        SendKeys.SendWait("^({p})");
                        break;
                    case "SAVE PAGE":
                        SendKeys.SendWait("^({s})");
                        break;
                    case "SOURCE CODE":
                        SendKeys.SendWait("^({u})");
                        break;
                    case "DEVELOPER TOOLS":
                        SendKeys.SendWait("{F12}");
                        break;
                    default:
                        break;
                }
            }

            if (!Onsearch)
            {
                foreach (KeyValuePair<string, string> pair in VoiceCommands)
                {
                    if (e.Result.Text.ToUpperInvariant().Equals(pair.Key.ToUpperInvariant()))
                    {
                        char[] input = pair.Key.ToCharArray();
                        foreach (char ch in input)
                        {
                            if (ch != ' ')
                                SendKeys.SendWait("{" + ch + "}");
                            else
                                SendKeys.SendWait(" ");
                        }
                        SendKeys.SendWait("{ENTER}");
                    }
                }
            }

            if (Onsearch)
            {
                foreach (string dkey in VSCmd)
                {
                    if (e.Result.Text.ToUpperInvariant().Equals(dkey.ToUpperInvariant()))
                    {
                        char[] inputs = dkey.ToCharArray();
                        foreach (char chs in inputs)
                        {
                            if (chs != ' ')
                                SendKeys.SendWait("{" + chs + "}");
                            else
                                SendKeys.SendWait(" ");
                        }
                        SendKeys.SendWait(" ");
                    }
                }
            }

        }

        public void AllVoiceCommands()
        {
            try
            {
                StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\urlCommands.txt");
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    String[] splitted = line.Split('=');
                    VoiceCommands.Add(splitted[0], splitted[1]);
                }
                sr.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\searchCommands.txt");
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    VSCmd.Add(line);
                }
                sr.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void KeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (!keybo)
            {
                keybo = true;
                Keyboard kbo = new Keyboard();
                kbo.Show();
            }
        }

        private void UrlVoiceCmd_Click(object sender, RoutedEventArgs e)
        {
            if (!urlbo)
            {
                urlbo = true;
                UrlVoiceCommands oform = new UrlVoiceCommands();
                oform.VoiceCommands = VoiceCommands;
                oform.Show();
            }
        }

        private void SearchVoiceCmd_Click(object sender, RoutedEventArgs e)
        {
            if (!searchbo)
            {
                searchbo = true;
                SearchVoiceCommands sform = new SearchVoiceCommands();
                sform.VSCmd = VSCmd;
                sform.Show();
            }
        }
    }
}
