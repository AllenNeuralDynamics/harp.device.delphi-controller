using Bonsai.Harp;
using System.Threading;
using System.Threading.Tasks;

namespace AllenNeuralDynamics.DelphiController
{
    /// <inheritdoc/>
    public partial class Device
    {
        /// <summary>
        /// Initializes a new instance of the asynchronous API to configure and interface
        /// with DelphiController devices on the specified serial port.
        /// </summary>
        /// <param name="portName">
        /// The name of the serial port used to communicate with the Harp device.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous initialization operation. The value of
        /// the <see cref="Task{TResult}.Result"/> parameter contains a new instance of
        /// the <see cref="AsyncDevice"/> class.
        /// </returns>
        public static async Task<AsyncDevice> CreateAsync(string portName, CancellationToken cancellationToken = default)
        {
            var device = new AsyncDevice(portName);
            var whoAmI = await device.ReadWhoAmIAsync(cancellationToken);
            if (whoAmI != Device.WhoAmI)
            {
                var errorMessage = string.Format(
                    "The device ID {1} on {0} was unexpected. Check whether a DelphiController device is connected to the specified serial port.",
                    portName, whoAmI);
                throw new HarpException(errorMessage);
            }

            return device;
        }
    }

    /// <summary>
    /// Represents an asynchronous API to configure and interface with DelphiController devices.
    /// </summary>
    public partial class AsyncDevice : Bonsai.Harp.AsyncDevice
    {
        internal AsyncDevice(string portName)
            : base(portName)
        {
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<ushort> ReadValveStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(ValveState.Address), cancellationToken);
            return ValveState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<ushort>> ReadTimestampedValveStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(ValveState.Address), cancellationToken);
            return ValveState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValvesSet register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<ValveMask> ReadValvesSetAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(ValvesSet.Address), cancellationToken);
            return ValvesSet.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValvesSet register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<ValveMask>> ReadTimestampedValvesSetAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(ValvesSet.Address), cancellationToken);
            return ValvesSet.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValvesSet register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValvesSetAsync(ValveMask value, CancellationToken cancellationToken = default)
        {
            var request = ValvesSet.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValvesClear register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<ValveMask> ReadValvesClearAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(ValvesClear.Address), cancellationToken);
            return ValvesClear.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValvesClear register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<ValveMask>> ReadTimestampedValvesClearAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(ValvesClear.Address), cancellationToken);
            return ValvesClear.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValvesClear register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValvesClearAsync(ValveMask value, CancellationToken cancellationToken = default)
        {
            var request = ValvesClear.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig0 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig0Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig0.Address), cancellationToken);
            return ValveConfig0.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig0 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig0Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig0.Address), cancellationToken);
            return ValveConfig0.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig0 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig0Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig0.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig1 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig1Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig1.Address), cancellationToken);
            return ValveConfig1.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig1 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig1Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig1.Address), cancellationToken);
            return ValveConfig1.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig1 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig1Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig1.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig2 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig2Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig2.Address), cancellationToken);
            return ValveConfig2.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig2 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig2Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig2.Address), cancellationToken);
            return ValveConfig2.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig2 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig2Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig2.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig3 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig3Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig3.Address), cancellationToken);
            return ValveConfig3.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig3 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig3Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig3.Address), cancellationToken);
            return ValveConfig3.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig3 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig3Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig3.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig4 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig4Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig4.Address), cancellationToken);
            return ValveConfig4.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig4 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig4Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig4.Address), cancellationToken);
            return ValveConfig4.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig4 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig4Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig4.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig5 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig5Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig5.Address), cancellationToken);
            return ValveConfig5.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig5 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig5Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig5.Address), cancellationToken);
            return ValveConfig5.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig5 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig5Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig5.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig6 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig6Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig6.Address), cancellationToken);
            return ValveConfig6.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig6 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig6Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig6.Address), cancellationToken);
            return ValveConfig6.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig6 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig6Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig6.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig7 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig7Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig7.Address), cancellationToken);
            return ValveConfig7.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig7 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig7Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig7.Address), cancellationToken);
            return ValveConfig7.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig7 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig7Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig7.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig8 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig8Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig8.Address), cancellationToken);
            return ValveConfig8.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig8 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig8Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig8.Address), cancellationToken);
            return ValveConfig8.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig8 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig8Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig8.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig9 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig9Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig9.Address), cancellationToken);
            return ValveConfig9.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig9 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig9Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig9.Address), cancellationToken);
            return ValveConfig9.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig9 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig9Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig9.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig10 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig10Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig10.Address), cancellationToken);
            return ValveConfig10.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig10 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig10Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig10.Address), cancellationToken);
            return ValveConfig10.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig10 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig10Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig10.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig11 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig11Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig11.Address), cancellationToken);
            return ValveConfig11.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig11 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig11Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig11.Address), cancellationToken);
            return ValveConfig11.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig11 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig11Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig11.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig12 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig12Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig12.Address), cancellationToken);
            return ValveConfig12.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig12 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig12Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig12.Address), cancellationToken);
            return ValveConfig12.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig12 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig12Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig12.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig13 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig13Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig13.Address), cancellationToken);
            return ValveConfig13.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig13 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig13Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig13.Address), cancellationToken);
            return ValveConfig13.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig13 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig13Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig13.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig14 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig14Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig14.Address), cancellationToken);
            return ValveConfig14.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig14 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig14Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig14.Address), cancellationToken);
            return ValveConfig14.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig14 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig14Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig14.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ValveConfig15 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte[]> ReadValveConfig15Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig15.Address), cancellationToken);
            return ValveConfig15.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ValveConfig15 register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedValveConfig15Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ValveConfig15.Address), cancellationToken);
            return ValveConfig15.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ValveConfig15 register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteValveConfig15Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = ValveConfig15.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the AuxGPIODir register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<AuxGPIOMask> ReadAuxGPIODirAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIODir.Address), cancellationToken);
            return AuxGPIODir.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the AuxGPIODir register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<AuxGPIOMask>> ReadTimestampedAuxGPIODirAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIODir.Address), cancellationToken);
            return AuxGPIODir.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the AuxGPIODir register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteAuxGPIODirAsync(AuxGPIOMask value, CancellationToken cancellationToken = default)
        {
            var request = AuxGPIODir.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the AuxGPIOState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<AuxGPIOMask> ReadAuxGPIOStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOState.Address), cancellationToken);
            return AuxGPIOState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the AuxGPIOState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<AuxGPIOMask>> ReadTimestampedAuxGPIOStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOState.Address), cancellationToken);
            return AuxGPIOState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the AuxGPIOState register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteAuxGPIOStateAsync(AuxGPIOMask value, CancellationToken cancellationToken = default)
        {
            var request = AuxGPIOState.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the AuxGPIOSet register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<AuxGPIOMask> ReadAuxGPIOSetAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOSet.Address), cancellationToken);
            return AuxGPIOSet.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the AuxGPIOSet register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<AuxGPIOMask>> ReadTimestampedAuxGPIOSetAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOSet.Address), cancellationToken);
            return AuxGPIOSet.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the AuxGPIOSet register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteAuxGPIOSetAsync(AuxGPIOMask value, CancellationToken cancellationToken = default)
        {
            var request = AuxGPIOSet.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the AuxGPIOClear register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<AuxGPIOMask> ReadAuxGPIOClearAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOClear.Address), cancellationToken);
            return AuxGPIOClear.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the AuxGPIOClear register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<AuxGPIOMask>> ReadTimestampedAuxGPIOClearAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOClear.Address), cancellationToken);
            return AuxGPIOClear.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the AuxGPIOClear register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteAuxGPIOClearAsync(AuxGPIOMask value, CancellationToken cancellationToken = default)
        {
            var request = AuxGPIOClear.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the AuxGPIOInputRiseEvent register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<AuxGPIOMask> ReadAuxGPIOInputRiseEventAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOInputRiseEvent.Address), cancellationToken);
            return AuxGPIOInputRiseEvent.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the AuxGPIOInputRiseEvent register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<AuxGPIOMask>> ReadTimestampedAuxGPIOInputRiseEventAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOInputRiseEvent.Address), cancellationToken);
            return AuxGPIOInputRiseEvent.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the AuxGPIOInputFallEvent register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<AuxGPIOMask> ReadAuxGPIOInputFallEventAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOInputFallEvent.Address), cancellationToken);
            return AuxGPIOInputFallEvent.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the AuxGPIOInputFallEvent register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<AuxGPIOMask>> ReadTimestampedAuxGPIOInputFallEventAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOInputFallEvent.Address), cancellationToken);
            return AuxGPIOInputFallEvent.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the AuxGPIORisingInputs register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<AuxGPIOMask> ReadAuxGPIORisingInputsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIORisingInputs.Address), cancellationToken);
            return AuxGPIORisingInputs.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the AuxGPIORisingInputs register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<AuxGPIOMask>> ReadTimestampedAuxGPIORisingInputsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIORisingInputs.Address), cancellationToken);
            return AuxGPIORisingInputs.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the AuxGPIORisingInputs register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteAuxGPIORisingInputsAsync(AuxGPIOMask value, CancellationToken cancellationToken = default)
        {
            var request = AuxGPIORisingInputs.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the AuxGPIOFallingInputs register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<AuxGPIOMask> ReadAuxGPIOFallingInputsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOFallingInputs.Address), cancellationToken);
            return AuxGPIOFallingInputs.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the AuxGPIOFallingInputs register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<AuxGPIOMask>> ReadTimestampedAuxGPIOFallingInputsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(AuxGPIOFallingInputs.Address), cancellationToken);
            return AuxGPIOFallingInputs.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the AuxGPIOFallingInputs register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteAuxGPIOFallingInputsAsync(AuxGPIOMask value, CancellationToken cancellationToken = default)
        {
            var request = AuxGPIOFallingInputs.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the PokePin register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadPokePinAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PokePin.Address), cancellationToken);
            return PokePin.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the PokePin register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedPokePinAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PokePin.Address), cancellationToken);
            return PokePin.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the PokePin register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePokePinAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = PokePin.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the PokePinInverted register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadPokePinInvertedAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PokePinInverted.Address), cancellationToken);
            return PokePinInverted.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the PokePinInverted register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedPokePinInvertedAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PokePinInverted.Address), cancellationToken);
            return PokePinInverted.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the PokePinInverted register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePokePinInvertedAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = PokePinInverted.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the PokeState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadPokeStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PokeState.Address), cancellationToken);
            return PokeState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the PokeState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedPokeStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PokeState.Address), cancellationToken);
            return PokeState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the RawPokeState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadRawPokeStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(RawPokeState.Address), cancellationToken);
            return RawPokeState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the RawPokeState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedRawPokeStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(RawPokeState.Address), cancellationToken);
            return RawPokeState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the PokeDometer register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<uint> ReadPokeDometerAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(PokeDometer.Address), cancellationToken);
            return PokeDometer.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the PokeDometer register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<uint>> ReadTimestampedPokeDometerAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(PokeDometer.Address), cancellationToken);
            return PokeDometer.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the FSMState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadFSMStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(FSMState.Address), cancellationToken);
            return FSMState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the FSMState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedFSMStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(FSMState.Address), cancellationToken);
            return FSMState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the FSMState register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteFSMStateAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = FSMState.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ForceFSM register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadForceFSMAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ForceFSM.Address), cancellationToken);
            return ForceFSM.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ForceFSM register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedForceFSMAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ForceFSM.Address), cancellationToken);
            return ForceFSM.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ForceFSM register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteForceFSMAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = ForceFSM.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the QueuedOdorMask register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<ushort> ReadQueuedOdorMaskAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(QueuedOdorMask.Address), cancellationToken);
            return QueuedOdorMask.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the QueuedOdorMask register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<ushort>> ReadTimestampedQueuedOdorMaskAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(QueuedOdorMask.Address), cancellationToken);
            return QueuedOdorMask.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the OdorSetupTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<uint> ReadOdorSetupTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(OdorSetupTimeUS.Address), cancellationToken);
            return OdorSetupTimeUS.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the OdorSetupTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<uint>> ReadTimestampedOdorSetupTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(OdorSetupTimeUS.Address), cancellationToken);
            return OdorSetupTimeUS.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the OdorSetupTimeUS register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteOdorSetupTimeUSAsync(uint value, CancellationToken cancellationToken = default)
        {
            var request = OdorSetupTimeUS.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the MinOdorDeliveryTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<uint> ReadMinOdorDeliveryTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(MinOdorDeliveryTimeUS.Address), cancellationToken);
            return MinOdorDeliveryTimeUS.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the MinOdorDeliveryTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<uint>> ReadTimestampedMinOdorDeliveryTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(MinOdorDeliveryTimeUS.Address), cancellationToken);
            return MinOdorDeliveryTimeUS.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the MinOdorDeliveryTimeUS register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteMinOdorDeliveryTimeUSAsync(uint value, CancellationToken cancellationToken = default)
        {
            var request = MinOdorDeliveryTimeUS.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the MaxOdorDeliveryTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<uint> ReadMaxOdorDeliveryTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(MaxOdorDeliveryTimeUS.Address), cancellationToken);
            return MaxOdorDeliveryTimeUS.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the MaxOdorDeliveryTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<uint>> ReadTimestampedMaxOdorDeliveryTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(MaxOdorDeliveryTimeUS.Address), cancellationToken);
            return MaxOdorDeliveryTimeUS.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the MaxOdorDeliveryTimeUS register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteMaxOdorDeliveryTimeUSAsync(uint value, CancellationToken cancellationToken = default)
        {
            var request = MaxOdorDeliveryTimeUS.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the MinimumPokeTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<uint> ReadMinimumPokeTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(MinimumPokeTimeUS.Address), cancellationToken);
            return MinimumPokeTimeUS.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the MinimumPokeTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<uint>> ReadTimestampedMinimumPokeTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(MinimumPokeTimeUS.Address), cancellationToken);
            return MinimumPokeTimeUS.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the MinimumPokeTimeUS register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteMinimumPokeTimeUSAsync(uint value, CancellationToken cancellationToken = default)
        {
            var request = MinimumPokeTimeUS.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the OdorDwellTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<uint> ReadOdorDwellTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(OdorDwellTimeUS.Address), cancellationToken);
            return OdorDwellTimeUS.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the OdorDwellTimeUS register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<uint>> ReadTimestampedOdorDwellTimeUSAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(OdorDwellTimeUS.Address), cancellationToken);
            return OdorDwellTimeUS.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the OdorDwellTimeUS register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteOdorDwellTimeUSAsync(uint value, CancellationToken cancellationToken = default)
        {
            var request = OdorDwellTimeUS.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the Cam0PinState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadCam0PinStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(Cam0PinState.Address), cancellationToken);
            return Cam0PinState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the Cam0PinState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedCam0PinStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(Cam0PinState.Address), cancellationToken);
            return Cam0PinState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the Cam0FrameRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<uint> ReadCam0FrameRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(Cam0FrameRate.Address), cancellationToken);
            return Cam0FrameRate.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the Cam0FrameRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<uint>> ReadTimestampedCam0FrameRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(Cam0FrameRate.Address), cancellationToken);
            return Cam0FrameRate.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the Cam0FrameRate register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteCam0FrameRateAsync(uint value, CancellationToken cancellationToken = default)
        {
            var request = Cam0FrameRate.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the Cam0DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadCam0DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(Cam0DutyCycle.Address), cancellationToken);
            return Cam0DutyCycle.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the Cam0DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedCam0DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(Cam0DutyCycle.Address), cancellationToken);
            return Cam0DutyCycle.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the Cam0DutyCycle register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteCam0DutyCycleAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = Cam0DutyCycle.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the EnableCam0Trigger register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadEnableCam0TriggerAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableCam0Trigger.Address), cancellationToken);
            return EnableCam0Trigger.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the EnableCam0Trigger register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedEnableCam0TriggerAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableCam0Trigger.Address), cancellationToken);
            return EnableCam0Trigger.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the EnableCam0Trigger register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteEnableCam0TriggerAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = EnableCam0Trigger.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the Cam1PinState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadCam1PinStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(Cam1PinState.Address), cancellationToken);
            return Cam1PinState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the Cam1PinState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedCam1PinStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(Cam1PinState.Address), cancellationToken);
            return Cam1PinState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the Cam1FrameRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<uint> ReadCam1FrameRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(Cam1FrameRate.Address), cancellationToken);
            return Cam1FrameRate.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the Cam1FrameRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<uint>> ReadTimestampedCam1FrameRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(Cam1FrameRate.Address), cancellationToken);
            return Cam1FrameRate.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the Cam1FrameRate register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteCam1FrameRateAsync(uint value, CancellationToken cancellationToken = default)
        {
            var request = Cam1FrameRate.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the Cam1DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadCam1DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(Cam1DutyCycle.Address), cancellationToken);
            return Cam1DutyCycle.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the Cam1DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedCam1DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(Cam1DutyCycle.Address), cancellationToken);
            return Cam1DutyCycle.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the Cam1DutyCycle register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteCam1DutyCycleAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = Cam1DutyCycle.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the EnableCam1Trigger register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadEnableCam1TriggerAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableCam1Trigger.Address), cancellationToken);
            return EnableCam1Trigger.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the EnableCam1Trigger register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedEnableCam1TriggerAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableCam1Trigger.Address), cancellationToken);
            return EnableCam1Trigger.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the EnableCam1Trigger register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteEnableCam1TriggerAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = EnableCam1Trigger.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the EnableValveLeds register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadEnableValveLedsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableValveLeds.Address), cancellationToken);
            return EnableValveLeds.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the EnableValveLeds register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedEnableValveLedsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableValveLeds.Address), cancellationToken);
            return EnableValveLeds.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the EnableValveLeds register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteEnableValveLedsAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = EnableValveLeds.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the LatestFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<LatestFlowRatePayload> ReadLatestFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(LatestFlowRate.Address), cancellationToken);
            return LatestFlowRate.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the LatestFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<LatestFlowRatePayload>> ReadTimestampedLatestFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(LatestFlowRate.Address), cancellationToken);
            return LatestFlowRate.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the LatestRawAdcSample register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<LatestRawAdcSamplePayload> ReadLatestRawAdcSampleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(LatestRawAdcSample.Address), cancellationToken);
            return LatestRawAdcSample.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the LatestRawAdcSample register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<LatestRawAdcSamplePayload>> ReadTimestampedLatestRawAdcSampleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(LatestRawAdcSample.Address), cancellationToken);
            return LatestRawAdcSample.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the EnableAdcSampling register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadEnableAdcSamplingAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableAdcSampling.Address), cancellationToken);
            return EnableAdcSampling.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the EnableAdcSampling register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedEnableAdcSamplingAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableAdcSampling.Address), cancellationToken);
            return EnableAdcSampling.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the EnableAdcSampling register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteEnableAdcSamplingAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = EnableAdcSampling.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the LeakAdcChannel register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<sbyte> ReadLeakAdcChannelAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSByte(LeakAdcChannel.Address), cancellationToken);
            return LeakAdcChannel.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the LeakAdcChannel register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<sbyte>> ReadTimestampedLeakAdcChannelAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSByte(LeakAdcChannel.Address), cancellationToken);
            return LeakAdcChannel.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the LeakAdcChannel register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteLeakAdcChannelAsync(sbyte value, CancellationToken cancellationToken = default)
        {
            var request = LeakAdcChannel.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the LeakThreshold register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadLeakThresholdAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(LeakThreshold.Address), cancellationToken);
            return LeakThreshold.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the LeakThreshold register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedLeakThresholdAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(LeakThreshold.Address), cancellationToken);
            return LeakThreshold.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the LeakThreshold register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteLeakThresholdAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = LeakThreshold.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the LeakState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadLeakStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(LeakState.Address), cancellationToken);
            return LeakState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the LeakState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedLeakStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(LeakState.Address), cancellationToken);
            return LeakState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ManualFlowMeter register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<sbyte> ReadManualFlowMeterAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSByte(ManualFlowMeter.Address), cancellationToken);
            return ManualFlowMeter.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ManualFlowMeter register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<sbyte>> ReadTimestampedManualFlowMeterAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSByte(ManualFlowMeter.Address), cancellationToken);
            return ManualFlowMeter.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ManualFlowMeter register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteManualFlowMeterAsync(sbyte value, CancellationToken cancellationToken = default)
        {
            var request = ManualFlowMeter.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the NominalFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadNominalFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(NominalFlowRate.Address), cancellationToken);
            return NominalFlowRate.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the NominalFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedNominalFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(NominalFlowRate.Address), cancellationToken);
            return NominalFlowRate.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the NominalFlowRate register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteNominalFlowRateAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = NominalFlowRate.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the FlowRateTolerance register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadFlowRateToleranceAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(FlowRateTolerance.Address), cancellationToken);
            return FlowRateTolerance.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the FlowRateTolerance register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedFlowRateToleranceAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(FlowRateTolerance.Address), cancellationToken);
            return FlowRateTolerance.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the FlowRateTolerance register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteFlowRateToleranceAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = FlowRateTolerance.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ManualFlowMeterState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadManualFlowMeterStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ManualFlowMeterState.Address), cancellationToken);
            return ManualFlowMeterState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ManualFlowMeterState register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedManualFlowMeterStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ManualFlowMeterState.Address), cancellationToken);
            return ManualFlowMeterState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the FlowMeterCalibrations register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<FlowMeterCalibrationsPayload> ReadFlowMeterCalibrationsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(FlowMeterCalibrations.Address), cancellationToken);
            return FlowMeterCalibrations.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the FlowMeterCalibrations register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<FlowMeterCalibrationsPayload>> ReadTimestampedFlowMeterCalibrationsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(FlowMeterCalibrations.Address), cancellationToken);
            return FlowMeterCalibrations.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the FlowMeterCalibrations register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteFlowMeterCalibrationsAsync(FlowMeterCalibrationsPayload value, CancellationToken cancellationToken = default)
        {
            var request = FlowMeterCalibrations.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the PidUpdateFrequency register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadPidUpdateFrequencyAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(PidUpdateFrequency.Address), cancellationToken);
            return PidUpdateFrequency.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the PidUpdateFrequency register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedPidUpdateFrequencyAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(PidUpdateFrequency.Address), cancellationToken);
            return PidUpdateFrequency.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the PidUpdateFrequency register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePidUpdateFrequencyAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = PidUpdateFrequency.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the PidGains register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<PidGainsPayload> ReadPidGainsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(PidGains.Address), cancellationToken);
            return PidGains.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the PidGains register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<PidGainsPayload>> ReadTimestampedPidGainsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(PidGains.Address), cancellationToken);
            return PidGains.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the PidGains register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePidGainsAsync(PidGainsPayload value, CancellationToken cancellationToken = default)
        {
            var request = PidGains.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve0Adc register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadProportionalValve0AdcAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve0Adc.Address), cancellationToken);
            return ProportionalValve0Adc.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve0Adc register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedProportionalValve0AdcAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve0Adc.Address), cancellationToken);
            return ProportionalValve0Adc.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve0Adc register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve0AdcAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve0Adc.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve0EnablePid register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadProportionalValve0EnablePidAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve0EnablePid.Address), cancellationToken);
            return ProportionalValve0EnablePid.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve0EnablePid register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedProportionalValve0EnablePidAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve0EnablePid.Address), cancellationToken);
            return ProportionalValve0EnablePid.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve0EnablePid register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve0EnablePidAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve0EnablePid.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve0DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadProportionalValve0DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve0DutyCycle.Address), cancellationToken);
            return ProportionalValve0DutyCycle.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve0DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedProportionalValve0DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve0DutyCycle.Address), cancellationToken);
            return ProportionalValve0DutyCycle.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve0DutyCycle register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve0DutyCycleAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve0DutyCycle.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve0TargetFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadProportionalValve0TargetFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve0TargetFlowRate.Address), cancellationToken);
            return ProportionalValve0TargetFlowRate.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve0TargetFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedProportionalValve0TargetFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve0TargetFlowRate.Address), cancellationToken);
            return ProportionalValve0TargetFlowRate.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve0TargetFlowRate register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve0TargetFlowRateAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve0TargetFlowRate.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve1Adc register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadProportionalValve1AdcAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve1Adc.Address), cancellationToken);
            return ProportionalValve1Adc.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve1Adc register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedProportionalValve1AdcAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve1Adc.Address), cancellationToken);
            return ProportionalValve1Adc.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve1Adc register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve1AdcAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve1Adc.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve1EnablePid register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadProportionalValve1EnablePidAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve1EnablePid.Address), cancellationToken);
            return ProportionalValve1EnablePid.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve1EnablePid register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedProportionalValve1EnablePidAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve1EnablePid.Address), cancellationToken);
            return ProportionalValve1EnablePid.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve1EnablePid register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve1EnablePidAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve1EnablePid.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve1DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadProportionalValve1DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve1DutyCycle.Address), cancellationToken);
            return ProportionalValve1DutyCycle.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve1DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedProportionalValve1DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve1DutyCycle.Address), cancellationToken);
            return ProportionalValve1DutyCycle.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve1DutyCycle register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve1DutyCycleAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve1DutyCycle.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve1TargetFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadProportionalValve1TargetFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve1TargetFlowRate.Address), cancellationToken);
            return ProportionalValve1TargetFlowRate.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve1TargetFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedProportionalValve1TargetFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve1TargetFlowRate.Address), cancellationToken);
            return ProportionalValve1TargetFlowRate.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve1TargetFlowRate register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve1TargetFlowRateAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve1TargetFlowRate.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve2Adc register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadProportionalValve2AdcAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve2Adc.Address), cancellationToken);
            return ProportionalValve2Adc.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve2Adc register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedProportionalValve2AdcAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve2Adc.Address), cancellationToken);
            return ProportionalValve2Adc.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve2Adc register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve2AdcAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve2Adc.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve2EnablePid register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadProportionalValve2EnablePidAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve2EnablePid.Address), cancellationToken);
            return ProportionalValve2EnablePid.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve2EnablePid register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedProportionalValve2EnablePidAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(ProportionalValve2EnablePid.Address), cancellationToken);
            return ProportionalValve2EnablePid.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve2EnablePid register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve2EnablePidAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve2EnablePid.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve2DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadProportionalValve2DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve2DutyCycle.Address), cancellationToken);
            return ProportionalValve2DutyCycle.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve2DutyCycle register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedProportionalValve2DutyCycleAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve2DutyCycle.Address), cancellationToken);
            return ProportionalValve2DutyCycle.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve2DutyCycle register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve2DutyCycleAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve2DutyCycle.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ProportionalValve2TargetFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<float> ReadProportionalValve2TargetFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve2TargetFlowRate.Address), cancellationToken);
            return ProportionalValve2TargetFlowRate.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the ProportionalValve2TargetFlowRate register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<float>> ReadTimestampedProportionalValve2TargetFlowRateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(ProportionalValve2TargetFlowRate.Address), cancellationToken);
            return ProportionalValve2TargetFlowRate.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the ProportionalValve2TargetFlowRate register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteProportionalValve2TargetFlowRateAsync(float value, CancellationToken cancellationToken = default)
        {
            var request = ProportionalValve2TargetFlowRate.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the FreezePidUpdates register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<byte> ReadFreezePidUpdatesAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(FreezePidUpdates.Address), cancellationToken);
            return FreezePidUpdates.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the FreezePidUpdates register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte>> ReadTimestampedFreezePidUpdatesAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(FreezePidUpdates.Address), cancellationToken);
            return FreezePidUpdates.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the FreezePidUpdates register.
        /// </summary>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteFreezePidUpdatesAsync(byte value, CancellationToken cancellationToken = default)
        {
            var request = FreezePidUpdates.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }
    }
}
