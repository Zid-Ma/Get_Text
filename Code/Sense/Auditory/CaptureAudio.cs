using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Accord;
using Accord.Audio;
using Accord.Audio.Filters;
using Accord.Audio.Formats;
using Accord.Audition.Beat;
using Accord.DirectSound;
using Accord.Imaging.Filters;
using Accord.Math;
using Microsoft.Win32;
using SharpDX.Multimedia;

namespace Get_Text
{
    internal class CaptureAudio
    {
        ArrayList audioDevices = new ArrayList();
        //音频源接口
        private IAudioSource source;
        //节拍检测器
        private EnergyBeatDetector detector;
        //离散信号值
        private Signal current;

        private MemoryStream stream;
        private IAudioOutput output;
        //波编码器
        private WaveEncoder encoder;
        //波解码器
        private WaveDecoder decoder;

        private float[] currentBuffer;
        ArrayList FramesBuffer = new ArrayList();

        private int frames;
        //音频样本数
        private int samples;
        private TimeSpan duration;

        //获取录音设备
        public bool GetAudioDevices()
        {
            audioDevices.Clear();
            // enumerate audio devices and add all devices to combo
            var ads = new AudioDeviceCollection(AudioDeviceCategory.Capture);
            foreach (var device in ads)
            {
                audioDevices.Add(device);
            }
            if(audioDevices.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //开启音频数据获取
        public void StartAudioData(int mothod = 0)
        {
            //假如录音设备数量大于0
            if (audioDevices.Count > 0)
            {
                //默认选择第一个音频设备,且读取音频设备信息
                AudioDeviceInfo info = audioDevices[0] as AudioDeviceInfo;
                if (mothod == 0)
                {
                    //创建一个音频源接口
                    source = new AudioCaptureDevice(info.Guid)
                    {
                        //设置每一帧的样本数量
                        DesiredFrameSize = 4096,//2048,
                                                //设置声音的采样率
                        SampleRate = 44100,//22050,

                        // We will be reading 16-bit PCM
                        Format = SampleFormat.Format16Bit
                    };
                }
                else
                {
                    //创建一个音频源接口
                    source = new AudioCaptureDevice(info.Guid)
                    {
                        //设置每一帧的样本数量
                        DesiredFrameSize = 1024,//2048,
                                                //设置声音的采样率
                        SampleRate = 22050,//22050,

                        // We will be reading 16-bit PCM
                        Format = SampleFormat.Format16Bit
                    };
                }
                
                source.NewFrame += source_NewFrame;
                source.AudioSourceError += source_AudioSourceError;

                //创建一个缓冲用于波形图的生成
                currentBuffer = new float[source.DesiredFrameSize];
                //创建一个内存流用于保存文件
                stream = new MemoryStream();
                encoder = new WaveEncoder(stream);

                //创建一个基于能量的节拍器
                detector = new EnergyBeatDetector(43);
                detector.Beat += new EventHandler(detector_Beat);
                //开始运行音频设备，并实施监听
                source.Start();
            }
            else
            {
                MessageBox.Show("未找到音频设备-麦克风");
            }
        }
        //保存文件
        public void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            Stream fileStream = saveFileDialog.OpenFile();
            stream.WriteTo(fileStream);
            fileStream.Close();
        }
        //播放存入的声音文件
        public void Play(IntPtr handle)
        {
            //首先,倒带这个流
            stream.Seek(0, SeekOrigin.Begin);
            //创建一个解码器
            decoder = new WaveDecoder(stream);
            //设置要导航到的示例的索引
            decoder.Seek(0);
            //获取波流的样本数
            int samplesLength = decoder.Samples;
            //创建一个播放音频的输出设备
            output = new AudioOutputDevice(handle, decoder.SampleRate, decoder.Channels);
            //安排一些事件
            output.FramePlayingStarted += output_FramePlayingStarted;
            output.NewFrameRequested += output_NewFrameRequested;
            output.Stopped += output_PlayingFinished;
            // 开始播放
            output.Play();
        }
        public void Stop()
        {
            // 停止这两种情况
            if (source != null)
            {
                // 如果在记录中
                source.SignalToStop();
                source.WaitForStop();
            }
            if (output != null)
            {
                // 如果在播放中
                output.SignalToStop();
                output.WaitForStop();
            }
            // 将缓冲区归零
            Array.Clear(currentBuffer, 0, currentBuffer.Length);
            // 将波形图归零
            updateWaveform(currentBuffer, currentBuffer.Length);
        }
        private int GetCurrentMicVolume()
        {
            int volume = 0;
            AudioDeviceInfo info = audioDevices[0] as AudioDeviceInfo;
           
            return volume;
        }
        //调整音量
        private void adjustVolume(float value)
        {
            // First, we rewind the stream
            stream.Seek(0, SeekOrigin.Begin);
            // Then we create a decoder for it
            decoder = new WaveDecoder(stream);
            var signal = decoder.Decode();
            // We apply the volume filter
            var volume = new VolumeFilter(value);
            volume.ApplyInPlace(signal);
            // Then we store it again
            stream.Seek(0, SeekOrigin.Begin);
            encoder = new WaveEncoder(stream);
            encoder.Encode(signal);
        }
        /// <summary>
        ///   在窗体图像中更新波形图
        ///   Updates the audio display in the wave chart
        /// </summary>
        /// 
        private void updateWaveform(float[] samples, int length)
        {
            //if (InvokeRequired)
            //{
            //    BeginInvoke(new Action(() =>
            //    {
            //        chart.UpdateWaveform("wave", samples, length);
            //    }));
            //}
            //else
            //{
            //    chart.UpdateWaveform("wave", current, length);
            //}
        }
        //节拍器回调函数
        void detector_Beat(object sender, EventArgs e)
        {

        }
        //当音频源监听到新的音频帧时，调用此函数
        void source_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //保存当前帧数据到缓冲对象
            eventArgs.Signal.CopyTo(currentBuffer);
            if (FramesBuffer.Count < 100)
            {
                //为音频帧加入信号
                FramesBuffer.Add(currentBuffer);
            }
            else
            {
                FramesBuffer.Remove(0);
                FramesBuffer.Add(currentBuffer);
            }
            //获取当前信号
            current = eventArgs.Signal;
            //检测给定信号中是否有节拍
            detector.Detect(current);
            //保存当前信号编码到内存
            encoder.Encode(eventArgs.Signal);
            //更新计数器
            duration += eventArgs.Signal.Duration;
            //获取音频样本数
            samples += eventArgs.Signal.Samples;
            //获取该信号的每个通道中的采样数，称为采样数
            frames += eventArgs.Signal.Length;
        }
        /// <summary>
        /// 这个回调将在音频出现错误时被调用
        ///   This callback will be called when there is some error with the audio 
        ///   source. It can be used to route exceptions so they don't compromise 
        ///   the audio processing pipeline.
        /// </summary>
        private void source_AudioSourceError(object sender, AudioSourceErrorEventArgs e)
        {
            throw new Exception(e.Description);
        }
        /// <summary>
        ///   当音频开始播放时，此事件将被触发
        ///   电脑扬声器。它可以用来更新UI，并很快发出通知
        ///   我们将请求额外的帧。
        /// </summary>
        /// 
        private void output_FramePlayingStarted(object sender, PlayFrameEventArgs e)
        {
            //获取当前帧位置索引
            int index = e.FrameIndex;
            //获取将要播放的帧数
            int count = e.Count;

            if (e.FrameIndex + e.Count < decoder.Frames)
            {
                //获取当前解码器流中的当前帧位置
                int previous = decoder.Position;
                //更改解码器流中的当前帧位置
                decoder.Seek(e.FrameIndex);
                //将波形流解码为信号对象
                Signal s = decoder.Decode(e.Count);
                //重新设置解码器流中的当前帧位置
                decoder.Seek(previous);
                //更新波形图
                updateWaveform(s.ToFloat(), s.Length);
            }
        }
        /// <summary>
        ///   当输出设备完成时触发此事件
        ///   播放音频流。同样，我们可以用它来更新UI。
        /// </summary>
        /// 
        private void output_PlayingFinished(object sender, EventArgs e)
        {
            //updateButtons();
            //清空数组对象
            Array.Clear(currentBuffer, 0, currentBuffer.Length);
            updateWaveform(currentBuffer, currentBuffer.Length);
        }
        /// <summary>
        /// 当声卡需要更多采样时触发此事件，当这种情况发生时，我们必须给它额外的帧，这样它才可以继续播放。
        ///   This event is triggered when the sound card needs more samples to be
        ///   played. When this happens, we have to feed it additional frames so it
        ///   can continue playing.
        /// </summary>
        private void output_NewFrameRequested(object sender, NewFrameRequestedEventArgs e)
        {
            // 获取下一个坐标系索引
            e.FrameIndex = decoder.Position;

            // 尝试解码来自流的请求帧数
            Signal signal = decoder.Decode(e.Frames);
            //如果我们无法获得请求的帧数
            if (signal == null)
            {
                //当如果发生这种情况，则表示我们需要停止。
                e.Stop = true;
                return;
            }
            try
            {
                // 通知帧数
                // 实际从源读取
                e.Frames = signal.Length;
                // 将信号复制到缓冲区
                signal.CopyTo(e.Buffer);
            }
            catch(Exception ex)
            {

            }
        }
    }
}
