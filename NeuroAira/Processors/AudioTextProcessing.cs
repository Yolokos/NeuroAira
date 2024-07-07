using Concentus.Oggfile;
using Concentus.Structs;
using NAudio.Wave;
using NeuroAira.Response;
using NeuroAira.Telegram;
using OpenAI;
using OpenAI.Audio;

namespace NeuroAira.Processors
{
	public class AudioTextProcessing : IDisposable
	{
		public AudioTextProcessing ()
		{
		}


		public async Task<AudioTextResponse<string>> ConvertVoiceToText (string voicePath)
		{
			try
			{
				using var api = new OpenAIClient("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");
				var request = new AudioTranscriptionRequest(Path.GetFullPath(voicePath), language: "ru");
				var response = await api.AudioEndpoint.CreateTranscriptionAsync(request);

				return new AudioTextResponse<string> { Data = response, Message = "Success convert voice to text", Status = true };
			}
			catch
			{
				return new AudioTextResponse<string> { Data = null, Message = "Success convert voice to text", Status = false };
			}
		}

		public async Task<AudioTextResponse<string>> DownloadVoiceFile (string outputPath, string voicePath)
		{
			try
			{
				using (Stream fileStream = System.IO.File.Create(outputPath))
				{
					await TelegramBotManager.Client.DownloadFileAsync(
						filePath: voicePath,
						destination: fileStream);
				}

				return new AudioTextResponse<string> { Data = null, Message = "File downloaded", Status = true };
			}
			catch
			{
				return new AudioTextResponse<string> { Data = null, Message = "Internal error. Download file", Status = false };
			}
		}

		public async Task<AudioTextResponse<string>> ConvertOggToMp3 (string pathToOggFile, long telegramId)
		{
			string pathToMp3File = $"content\\{telegramId}_audio.mp3";
			try
			{
				using (FileStream fileIn = new FileStream(pathToOggFile, FileMode.Open))
				using (MemoryStream pcmStream = new MemoryStream())
				{
					OpusDecoder decoder = OpusDecoder.Create(48000, 1);
					OpusOggReadStream oggIn = new OpusOggReadStream(decoder, fileIn);
					while (oggIn.HasNextPacket)
					{
						short[] packet = oggIn.DecodeNextPacket();
						if (packet != null)
						{
							for (int i = 0; i < packet.Length; i++)
							{
								var bytes = BitConverter.GetBytes(packet[i]);
								pcmStream.Write(bytes, 0, bytes.Length);
							}
						}
					}
					pcmStream.Position = 0;
					var wavStream = new RawSourceWaveStream(pcmStream, new WaveFormat(48000, 1));
					var sampleProvider = wavStream.ToSampleProvider();
					WaveFileWriter.CreateWaveFile16(pathToMp3File, sampleProvider);
				}

				System.IO.File.Delete(pathToOggFile);

				return new AudioTextResponse<string> { Data = pathToMp3File, Message = "Converted ogg to mp3", Status = true };
			}
			catch
			{
				return new AudioTextResponse<string> { Data = null, Message = "Internal error. Audio message.", Status = false };
			}
		}

		private bool disposed = false;

		protected virtual void Dispose (bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
				}
			}
			this.disposed = true;
		}

		public void Dispose ()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
