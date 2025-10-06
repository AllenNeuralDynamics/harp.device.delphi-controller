using Bonsai;
using Bonsai.Harp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace AllenNeuralDynamics.DelphiController
{
    /// <summary>
    /// Generates events and processes commands for the DelphiController device connected
    /// at the specified serial port.
    /// </summary>
    [Combinator(MethodName = nameof(Generate))]
    [WorkflowElementCategory(ElementCategory.Source)]
    [Description("Generates events and processes commands for the DelphiController device.")]
    public partial class Device : Bonsai.Harp.Device, INamedElement
    {
        /// <summary>
        /// Represents the unique identity class of the <see cref="DelphiController"/> device.
        /// This field is constant.
        /// </summary>
        public const int WhoAmI = 1407;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device() : base(WhoAmI) { }

        string INamedElement.Name => nameof(DelphiController);

        /// <summary>
        /// Gets a read-only mapping from address to register type.
        /// </summary>
        public static new IReadOnlyDictionary<int, Type> RegisterMap { get; } = new Dictionary<int, Type>
            (Bonsai.Harp.Device.RegisterMap.ToDictionary(entry => entry.Key, entry => entry.Value))
        {
            { 32, typeof(ValveState) },
            { 33, typeof(ValvesSet) },
            { 34, typeof(ValvesClear) },
            { 35, typeof(ValveConfig0) },
            { 36, typeof(ValveConfig1) },
            { 37, typeof(ValveConfig2) },
            { 38, typeof(ValveConfig3) },
            { 39, typeof(ValveConfig4) },
            { 40, typeof(ValveConfig5) },
            { 41, typeof(ValveConfig6) },
            { 42, typeof(ValveConfig7) },
            { 43, typeof(ValveConfig8) },
            { 44, typeof(ValveConfig9) },
            { 45, typeof(ValveConfig10) },
            { 46, typeof(ValveConfig11) },
            { 47, typeof(ValveConfig12) },
            { 48, typeof(ValveConfig13) },
            { 49, typeof(ValveConfig14) },
            { 50, typeof(ValveConfig15) },
            { 51, typeof(AuxGPIODir) },
            { 52, typeof(AuxGPIOState) },
            { 53, typeof(AuxGPIOSet) },
            { 54, typeof(AuxGPIOClear) },
            { 55, typeof(AuxGPIOInputRiseEvent) },
            { 56, typeof(AuxGPIOInputFallEvent) },
            { 57, typeof(AuxGPIORisingInputs) },
            { 58, typeof(AuxGPIOFallingInputs) },
            { 59, typeof(PokePin) },
            { 60, typeof(PokePinInverted) },
            { 61, typeof(PokeState) },
            { 62, typeof(RawPokeState) },
            { 63, typeof(PokeDometer) },
            { 64, typeof(FSMState) },
            { 65, typeof(ForceFSM) },
            { 66, typeof(QueuedOdorIndex) },
            { 67, typeof(VacuumCloseTimeUS) },
            { 68, typeof(MinOdorDeliveryTimeUS) },
            { 69, typeof(MaxOdorDeliveryTimeUS) },
            { 70, typeof(OdorTransitionTimeUS) },
            { 71, typeof(VacuumSetupTimeUS) },
            { 72, typeof(FinalValveEnergizedTimeUS) },
            { 73, typeof(MinimumPokeTimeUS) },
            { 74, typeof(CamPin) },
            { 75, typeof(CamPinState) },
            { 76, typeof(FrameRate) },
            { 77, typeof(DutyCycle) },
            { 78, typeof(EnableCamTrigger) },
            { 79, typeof(EnableValveLeds) }
        };

        /// <summary>
        /// Gets the contents of the metadata file describing the <see cref="DelphiController"/>
        /// device registers.
        /// </summary>
        public static readonly string Metadata = GetDeviceMetadata();

        static string GetDeviceMetadata()
        {
            var deviceType = typeof(Device);
            using var metadataStream = deviceType.Assembly.GetManifestResourceStream($"{deviceType.Namespace}.device.yml");
            using var streamReader = new System.IO.StreamReader(metadataStream);
            return streamReader.ReadToEnd();
        }
    }

    /// <summary>
    /// Represents an operator that returns the contents of the metadata file
    /// describing the <see cref="DelphiController"/> device registers.
    /// </summary>
    [Description("Returns the contents of the metadata file describing the DelphiController device registers.")]
    public partial class GetDeviceMetadata : Source<string>
    {
        /// <summary>
        /// Returns an observable sequence with the contents of the metadata file
        /// describing the <see cref="DelphiController"/> device registers.
        /// </summary>
        /// <returns>
        /// A sequence with a single <see cref="string"/> object representing the
        /// contents of the metadata file.
        /// </returns>
        public override IObservable<string> Generate()
        {
            return Observable.Return(Device.Metadata);
        }
    }

    /// <summary>
    /// Represents an operator that groups the sequence of <see cref="DelphiController"/>" messages by register type.
    /// </summary>
    [Description("Groups the sequence of DelphiController messages by register type.")]
    public partial class GroupByRegister : Combinator<HarpMessage, IGroupedObservable<Type, HarpMessage>>
    {
        /// <summary>
        /// Groups an observable sequence of <see cref="DelphiController"/> messages
        /// by register type.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of observable groups, each of which corresponds to a unique
        /// <see cref="DelphiController"/> register.
        /// </returns>
        public override IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(message => Device.RegisterMap[message.Address]);
        }
    }

    /// <summary>
    /// Represents an operator that writes the sequence of <see cref="DelphiController"/>" messages
    /// to the standard Harp storage format.
    /// </summary>
    [Description("Writes the sequence of DelphiController messages to the standard Harp storage format.")]
    public partial class DeviceDataWriter : Sink<HarpMessage>, INamedElement
    {
        const string BinaryExtension = ".bin";
        const string MetadataFileName = "device.yml";
        readonly Bonsai.Harp.MessageWriter writer = new();

        string INamedElement.Name => nameof(DelphiController) + "DataWriter";

        /// <summary>
        /// Gets or sets the relative or absolute path on which to save the message data.
        /// </summary>
        [Description("The relative or absolute path of the directory on which to save the message data.")]
        [Editor("Bonsai.Design.SaveFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string Path
        {
            get => System.IO.Path.GetDirectoryName(writer.FileName);
            set => writer.FileName = System.IO.Path.Combine(value, nameof(DelphiController) + BinaryExtension);
        }

        /// <summary>
        /// Gets or sets a value indicating whether element writing should be buffered. If <see langword="true"/>,
        /// the write commands will be queued in memory as fast as possible and will be processed
        /// by the writer in a different thread. Otherwise, writing will be done in the same
        /// thread in which notifications arrive.
        /// </summary>
        [Description("Indicates whether writing should be buffered.")]
        public bool Buffered
        {
            get => writer.Buffered;
            set => writer.Buffered = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to overwrite the output file if it already exists.
        /// </summary>
        [Description("Indicates whether to overwrite the output file if it already exists.")]
        public bool Overwrite
        {
            get => writer.Overwrite;
            set => writer.Overwrite = value;
        }

        /// <summary>
        /// Gets or sets a value specifying how the message filter will use the matching criteria.
        /// </summary>
        [Description("Specifies how the message filter will use the matching criteria.")]
        public FilterType FilterType
        {
            get => writer.FilterType;
            set => writer.FilterType = value;
        }

        /// <summary>
        /// Gets or sets a value specifying the expected message type. If no value is
        /// specified, all messages will be accepted.
        /// </summary>
        [Description("Specifies the expected message type. If no value is specified, all messages will be accepted.")]
        public MessageType? MessageType
        {
            get => writer.MessageType;
            set => writer.MessageType = value;
        }

        private IObservable<TSource> WriteDeviceMetadata<TSource>(IObservable<TSource> source)
        {
            var basePath = Path;
            if (string.IsNullOrEmpty(basePath))
                return source;

            var metadataPath = System.IO.Path.Combine(basePath, MetadataFileName);
            return Observable.Create<TSource>(observer =>
            {
                Bonsai.IO.PathHelper.EnsureDirectory(metadataPath);
                if (System.IO.File.Exists(metadataPath) && !Overwrite)
                {
                    throw new System.IO.IOException(string.Format("The file '{0}' already exists.", metadataPath));
                }

                System.IO.File.WriteAllText(metadataPath, Device.Metadata);
                return source.SubscribeSafe(observer);
            });
        }

        /// <summary>
        /// Writes each Harp message in the sequence to the specified binary file, and the
        /// contents of the device metadata file to a separate text file.
        /// </summary>
        /// <param name="source">The sequence of messages to write to the file.</param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the
        /// messages to a raw binary file, and the contents of the device metadata file
        /// to a separate text file.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            return source.Publish(ps => ps.Merge(
                WriteDeviceMetadata(writer.Process(ps.GroupBy(message => message.Address)))
                .IgnoreElements()
                .Cast<HarpMessage>()));
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated from
        /// the common group register address. The contents of the device metadata file are
        /// written to a separate text file.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique register
        /// address.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the Harp
        /// messages in each group to the corresponding file, and the contents of the device
        /// metadata file to a separate text file.
        /// </returns>
        public IObservable<IGroupedObservable<int, HarpMessage>> Process(IObservable<IGroupedObservable<int, HarpMessage>> source)
        {
            return WriteDeviceMetadata(writer.Process(source));
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated from
        /// the common group register name. The contents of the device metadata file are
        /// written to a separate text file.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique register
        /// type.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the Harp
        /// messages in each group to the corresponding file, and the contents of the device
        /// metadata file to a separate text file.
        /// </returns>
        public IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<IGroupedObservable<Type, HarpMessage>> source)
        {
            return WriteDeviceMetadata(writer.Process(source));
        }
    }

    /// <summary>
    /// Represents an operator that filters register-specific messages
    /// reported by the <see cref="DelphiController"/> device.
    /// </summary>
    /// <seealso cref="ValveState"/>
    /// <seealso cref="ValvesSet"/>
    /// <seealso cref="ValvesClear"/>
    /// <seealso cref="ValveConfig0"/>
    /// <seealso cref="ValveConfig1"/>
    /// <seealso cref="ValveConfig2"/>
    /// <seealso cref="ValveConfig3"/>
    /// <seealso cref="ValveConfig4"/>
    /// <seealso cref="ValveConfig5"/>
    /// <seealso cref="ValveConfig6"/>
    /// <seealso cref="ValveConfig7"/>
    /// <seealso cref="ValveConfig8"/>
    /// <seealso cref="ValveConfig9"/>
    /// <seealso cref="ValveConfig10"/>
    /// <seealso cref="ValveConfig11"/>
    /// <seealso cref="ValveConfig12"/>
    /// <seealso cref="ValveConfig13"/>
    /// <seealso cref="ValveConfig14"/>
    /// <seealso cref="ValveConfig15"/>
    /// <seealso cref="AuxGPIODir"/>
    /// <seealso cref="AuxGPIOState"/>
    /// <seealso cref="AuxGPIOSet"/>
    /// <seealso cref="AuxGPIOClear"/>
    /// <seealso cref="AuxGPIOInputRiseEvent"/>
    /// <seealso cref="AuxGPIOInputFallEvent"/>
    /// <seealso cref="AuxGPIORisingInputs"/>
    /// <seealso cref="AuxGPIOFallingInputs"/>
    /// <seealso cref="PokePin"/>
    /// <seealso cref="PokePinInverted"/>
    /// <seealso cref="PokeState"/>
    /// <seealso cref="RawPokeState"/>
    /// <seealso cref="PokeDometer"/>
    /// <seealso cref="FSMState"/>
    /// <seealso cref="ForceFSM"/>
    /// <seealso cref="QueuedOdorIndex"/>
    /// <seealso cref="VacuumCloseTimeUS"/>
    /// <seealso cref="MinOdorDeliveryTimeUS"/>
    /// <seealso cref="MaxOdorDeliveryTimeUS"/>
    /// <seealso cref="OdorTransitionTimeUS"/>
    /// <seealso cref="VacuumSetupTimeUS"/>
    /// <seealso cref="FinalValveEnergizedTimeUS"/>
    /// <seealso cref="MinimumPokeTimeUS"/>
    /// <seealso cref="CamPin"/>
    /// <seealso cref="CamPinState"/>
    /// <seealso cref="FrameRate"/>
    /// <seealso cref="DutyCycle"/>
    /// <seealso cref="EnableCamTrigger"/>
    /// <seealso cref="EnableValveLeds"/>
    [XmlInclude(typeof(ValveState))]
    [XmlInclude(typeof(ValvesSet))]
    [XmlInclude(typeof(ValvesClear))]
    [XmlInclude(typeof(ValveConfig0))]
    [XmlInclude(typeof(ValveConfig1))]
    [XmlInclude(typeof(ValveConfig2))]
    [XmlInclude(typeof(ValveConfig3))]
    [XmlInclude(typeof(ValveConfig4))]
    [XmlInclude(typeof(ValveConfig5))]
    [XmlInclude(typeof(ValveConfig6))]
    [XmlInclude(typeof(ValveConfig7))]
    [XmlInclude(typeof(ValveConfig8))]
    [XmlInclude(typeof(ValveConfig9))]
    [XmlInclude(typeof(ValveConfig10))]
    [XmlInclude(typeof(ValveConfig11))]
    [XmlInclude(typeof(ValveConfig12))]
    [XmlInclude(typeof(ValveConfig13))]
    [XmlInclude(typeof(ValveConfig14))]
    [XmlInclude(typeof(ValveConfig15))]
    [XmlInclude(typeof(AuxGPIODir))]
    [XmlInclude(typeof(AuxGPIOState))]
    [XmlInclude(typeof(AuxGPIOSet))]
    [XmlInclude(typeof(AuxGPIOClear))]
    [XmlInclude(typeof(AuxGPIOInputRiseEvent))]
    [XmlInclude(typeof(AuxGPIOInputFallEvent))]
    [XmlInclude(typeof(AuxGPIORisingInputs))]
    [XmlInclude(typeof(AuxGPIOFallingInputs))]
    [XmlInclude(typeof(PokePin))]
    [XmlInclude(typeof(PokePinInverted))]
    [XmlInclude(typeof(PokeState))]
    [XmlInclude(typeof(RawPokeState))]
    [XmlInclude(typeof(PokeDometer))]
    [XmlInclude(typeof(FSMState))]
    [XmlInclude(typeof(ForceFSM))]
    [XmlInclude(typeof(QueuedOdorIndex))]
    [XmlInclude(typeof(VacuumCloseTimeUS))]
    [XmlInclude(typeof(MinOdorDeliveryTimeUS))]
    [XmlInclude(typeof(MaxOdorDeliveryTimeUS))]
    [XmlInclude(typeof(OdorTransitionTimeUS))]
    [XmlInclude(typeof(VacuumSetupTimeUS))]
    [XmlInclude(typeof(FinalValveEnergizedTimeUS))]
    [XmlInclude(typeof(MinimumPokeTimeUS))]
    [XmlInclude(typeof(CamPin))]
    [XmlInclude(typeof(CamPinState))]
    [XmlInclude(typeof(FrameRate))]
    [XmlInclude(typeof(DutyCycle))]
    [XmlInclude(typeof(EnableCamTrigger))]
    [XmlInclude(typeof(EnableValveLeds))]
    [Description("Filters register-specific messages reported by the DelphiController device.")]
    public class FilterRegister : FilterRegisterBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegister"/> class.
        /// </summary>
        public FilterRegister()
        {
            Register = new ValveState();
        }

        string INamedElement.Name
        {
            get => $"{nameof(DelphiController)}.{GetElementDisplayName(Register)}";
        }
    }

    /// <summary>
    /// Represents an operator which filters and selects specific messages
    /// reported by the DelphiController device.
    /// </summary>
    /// <seealso cref="ValveState"/>
    /// <seealso cref="ValvesSet"/>
    /// <seealso cref="ValvesClear"/>
    /// <seealso cref="ValveConfig0"/>
    /// <seealso cref="ValveConfig1"/>
    /// <seealso cref="ValveConfig2"/>
    /// <seealso cref="ValveConfig3"/>
    /// <seealso cref="ValveConfig4"/>
    /// <seealso cref="ValveConfig5"/>
    /// <seealso cref="ValveConfig6"/>
    /// <seealso cref="ValveConfig7"/>
    /// <seealso cref="ValveConfig8"/>
    /// <seealso cref="ValveConfig9"/>
    /// <seealso cref="ValveConfig10"/>
    /// <seealso cref="ValveConfig11"/>
    /// <seealso cref="ValveConfig12"/>
    /// <seealso cref="ValveConfig13"/>
    /// <seealso cref="ValveConfig14"/>
    /// <seealso cref="ValveConfig15"/>
    /// <seealso cref="AuxGPIODir"/>
    /// <seealso cref="AuxGPIOState"/>
    /// <seealso cref="AuxGPIOSet"/>
    /// <seealso cref="AuxGPIOClear"/>
    /// <seealso cref="AuxGPIOInputRiseEvent"/>
    /// <seealso cref="AuxGPIOInputFallEvent"/>
    /// <seealso cref="AuxGPIORisingInputs"/>
    /// <seealso cref="AuxGPIOFallingInputs"/>
    /// <seealso cref="PokePin"/>
    /// <seealso cref="PokePinInverted"/>
    /// <seealso cref="PokeState"/>
    /// <seealso cref="RawPokeState"/>
    /// <seealso cref="PokeDometer"/>
    /// <seealso cref="FSMState"/>
    /// <seealso cref="ForceFSM"/>
    /// <seealso cref="QueuedOdorIndex"/>
    /// <seealso cref="VacuumCloseTimeUS"/>
    /// <seealso cref="MinOdorDeliveryTimeUS"/>
    /// <seealso cref="MaxOdorDeliveryTimeUS"/>
    /// <seealso cref="OdorTransitionTimeUS"/>
    /// <seealso cref="VacuumSetupTimeUS"/>
    /// <seealso cref="FinalValveEnergizedTimeUS"/>
    /// <seealso cref="MinimumPokeTimeUS"/>
    /// <seealso cref="CamPin"/>
    /// <seealso cref="CamPinState"/>
    /// <seealso cref="FrameRate"/>
    /// <seealso cref="DutyCycle"/>
    /// <seealso cref="EnableCamTrigger"/>
    /// <seealso cref="EnableValveLeds"/>
    [XmlInclude(typeof(ValveState))]
    [XmlInclude(typeof(ValvesSet))]
    [XmlInclude(typeof(ValvesClear))]
    [XmlInclude(typeof(ValveConfig0))]
    [XmlInclude(typeof(ValveConfig1))]
    [XmlInclude(typeof(ValveConfig2))]
    [XmlInclude(typeof(ValveConfig3))]
    [XmlInclude(typeof(ValveConfig4))]
    [XmlInclude(typeof(ValveConfig5))]
    [XmlInclude(typeof(ValveConfig6))]
    [XmlInclude(typeof(ValveConfig7))]
    [XmlInclude(typeof(ValveConfig8))]
    [XmlInclude(typeof(ValveConfig9))]
    [XmlInclude(typeof(ValveConfig10))]
    [XmlInclude(typeof(ValveConfig11))]
    [XmlInclude(typeof(ValveConfig12))]
    [XmlInclude(typeof(ValveConfig13))]
    [XmlInclude(typeof(ValveConfig14))]
    [XmlInclude(typeof(ValveConfig15))]
    [XmlInclude(typeof(AuxGPIODir))]
    [XmlInclude(typeof(AuxGPIOState))]
    [XmlInclude(typeof(AuxGPIOSet))]
    [XmlInclude(typeof(AuxGPIOClear))]
    [XmlInclude(typeof(AuxGPIOInputRiseEvent))]
    [XmlInclude(typeof(AuxGPIOInputFallEvent))]
    [XmlInclude(typeof(AuxGPIORisingInputs))]
    [XmlInclude(typeof(AuxGPIOFallingInputs))]
    [XmlInclude(typeof(PokePin))]
    [XmlInclude(typeof(PokePinInverted))]
    [XmlInclude(typeof(PokeState))]
    [XmlInclude(typeof(RawPokeState))]
    [XmlInclude(typeof(PokeDometer))]
    [XmlInclude(typeof(FSMState))]
    [XmlInclude(typeof(ForceFSM))]
    [XmlInclude(typeof(QueuedOdorIndex))]
    [XmlInclude(typeof(VacuumCloseTimeUS))]
    [XmlInclude(typeof(MinOdorDeliveryTimeUS))]
    [XmlInclude(typeof(MaxOdorDeliveryTimeUS))]
    [XmlInclude(typeof(OdorTransitionTimeUS))]
    [XmlInclude(typeof(VacuumSetupTimeUS))]
    [XmlInclude(typeof(FinalValveEnergizedTimeUS))]
    [XmlInclude(typeof(MinimumPokeTimeUS))]
    [XmlInclude(typeof(CamPin))]
    [XmlInclude(typeof(CamPinState))]
    [XmlInclude(typeof(FrameRate))]
    [XmlInclude(typeof(DutyCycle))]
    [XmlInclude(typeof(EnableCamTrigger))]
    [XmlInclude(typeof(EnableValveLeds))]
    [XmlInclude(typeof(TimestampedValveState))]
    [XmlInclude(typeof(TimestampedValvesSet))]
    [XmlInclude(typeof(TimestampedValvesClear))]
    [XmlInclude(typeof(TimestampedValveConfig0))]
    [XmlInclude(typeof(TimestampedValveConfig1))]
    [XmlInclude(typeof(TimestampedValveConfig2))]
    [XmlInclude(typeof(TimestampedValveConfig3))]
    [XmlInclude(typeof(TimestampedValveConfig4))]
    [XmlInclude(typeof(TimestampedValveConfig5))]
    [XmlInclude(typeof(TimestampedValveConfig6))]
    [XmlInclude(typeof(TimestampedValveConfig7))]
    [XmlInclude(typeof(TimestampedValveConfig8))]
    [XmlInclude(typeof(TimestampedValveConfig9))]
    [XmlInclude(typeof(TimestampedValveConfig10))]
    [XmlInclude(typeof(TimestampedValveConfig11))]
    [XmlInclude(typeof(TimestampedValveConfig12))]
    [XmlInclude(typeof(TimestampedValveConfig13))]
    [XmlInclude(typeof(TimestampedValveConfig14))]
    [XmlInclude(typeof(TimestampedValveConfig15))]
    [XmlInclude(typeof(TimestampedAuxGPIODir))]
    [XmlInclude(typeof(TimestampedAuxGPIOState))]
    [XmlInclude(typeof(TimestampedAuxGPIOSet))]
    [XmlInclude(typeof(TimestampedAuxGPIOClear))]
    [XmlInclude(typeof(TimestampedAuxGPIOInputRiseEvent))]
    [XmlInclude(typeof(TimestampedAuxGPIOInputFallEvent))]
    [XmlInclude(typeof(TimestampedAuxGPIORisingInputs))]
    [XmlInclude(typeof(TimestampedAuxGPIOFallingInputs))]
    [XmlInclude(typeof(TimestampedPokePin))]
    [XmlInclude(typeof(TimestampedPokePinInverted))]
    [XmlInclude(typeof(TimestampedPokeState))]
    [XmlInclude(typeof(TimestampedRawPokeState))]
    [XmlInclude(typeof(TimestampedPokeDometer))]
    [XmlInclude(typeof(TimestampedFSMState))]
    [XmlInclude(typeof(TimestampedForceFSM))]
    [XmlInclude(typeof(TimestampedQueuedOdorIndex))]
    [XmlInclude(typeof(TimestampedVacuumCloseTimeUS))]
    [XmlInclude(typeof(TimestampedMinOdorDeliveryTimeUS))]
    [XmlInclude(typeof(TimestampedMaxOdorDeliveryTimeUS))]
    [XmlInclude(typeof(TimestampedOdorTransitionTimeUS))]
    [XmlInclude(typeof(TimestampedVacuumSetupTimeUS))]
    [XmlInclude(typeof(TimestampedFinalValveEnergizedTimeUS))]
    [XmlInclude(typeof(TimestampedMinimumPokeTimeUS))]
    [XmlInclude(typeof(TimestampedCamPin))]
    [XmlInclude(typeof(TimestampedCamPinState))]
    [XmlInclude(typeof(TimestampedFrameRate))]
    [XmlInclude(typeof(TimestampedDutyCycle))]
    [XmlInclude(typeof(TimestampedEnableCamTrigger))]
    [XmlInclude(typeof(TimestampedEnableValveLeds))]
    [Description("Filters and selects specific messages reported by the DelphiController device.")]
    public partial class Parse : ParseBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Register = new ValveState();
        }

        string INamedElement.Name => $"{nameof(DelphiController)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents an operator which formats a sequence of values as specific
    /// DelphiController register messages.
    /// </summary>
    /// <seealso cref="ValveState"/>
    /// <seealso cref="ValvesSet"/>
    /// <seealso cref="ValvesClear"/>
    /// <seealso cref="ValveConfig0"/>
    /// <seealso cref="ValveConfig1"/>
    /// <seealso cref="ValveConfig2"/>
    /// <seealso cref="ValveConfig3"/>
    /// <seealso cref="ValveConfig4"/>
    /// <seealso cref="ValveConfig5"/>
    /// <seealso cref="ValveConfig6"/>
    /// <seealso cref="ValveConfig7"/>
    /// <seealso cref="ValveConfig8"/>
    /// <seealso cref="ValveConfig9"/>
    /// <seealso cref="ValveConfig10"/>
    /// <seealso cref="ValveConfig11"/>
    /// <seealso cref="ValveConfig12"/>
    /// <seealso cref="ValveConfig13"/>
    /// <seealso cref="ValveConfig14"/>
    /// <seealso cref="ValveConfig15"/>
    /// <seealso cref="AuxGPIODir"/>
    /// <seealso cref="AuxGPIOState"/>
    /// <seealso cref="AuxGPIOSet"/>
    /// <seealso cref="AuxGPIOClear"/>
    /// <seealso cref="AuxGPIOInputRiseEvent"/>
    /// <seealso cref="AuxGPIOInputFallEvent"/>
    /// <seealso cref="AuxGPIORisingInputs"/>
    /// <seealso cref="AuxGPIOFallingInputs"/>
    /// <seealso cref="PokePin"/>
    /// <seealso cref="PokePinInverted"/>
    /// <seealso cref="PokeState"/>
    /// <seealso cref="RawPokeState"/>
    /// <seealso cref="PokeDometer"/>
    /// <seealso cref="FSMState"/>
    /// <seealso cref="ForceFSM"/>
    /// <seealso cref="QueuedOdorIndex"/>
    /// <seealso cref="VacuumCloseTimeUS"/>
    /// <seealso cref="MinOdorDeliveryTimeUS"/>
    /// <seealso cref="MaxOdorDeliveryTimeUS"/>
    /// <seealso cref="OdorTransitionTimeUS"/>
    /// <seealso cref="VacuumSetupTimeUS"/>
    /// <seealso cref="FinalValveEnergizedTimeUS"/>
    /// <seealso cref="MinimumPokeTimeUS"/>
    /// <seealso cref="CamPin"/>
    /// <seealso cref="CamPinState"/>
    /// <seealso cref="FrameRate"/>
    /// <seealso cref="DutyCycle"/>
    /// <seealso cref="EnableCamTrigger"/>
    /// <seealso cref="EnableValveLeds"/>
    [XmlInclude(typeof(ValveState))]
    [XmlInclude(typeof(ValvesSet))]
    [XmlInclude(typeof(ValvesClear))]
    [XmlInclude(typeof(ValveConfig0))]
    [XmlInclude(typeof(ValveConfig1))]
    [XmlInclude(typeof(ValveConfig2))]
    [XmlInclude(typeof(ValveConfig3))]
    [XmlInclude(typeof(ValveConfig4))]
    [XmlInclude(typeof(ValveConfig5))]
    [XmlInclude(typeof(ValveConfig6))]
    [XmlInclude(typeof(ValveConfig7))]
    [XmlInclude(typeof(ValveConfig8))]
    [XmlInclude(typeof(ValveConfig9))]
    [XmlInclude(typeof(ValveConfig10))]
    [XmlInclude(typeof(ValveConfig11))]
    [XmlInclude(typeof(ValveConfig12))]
    [XmlInclude(typeof(ValveConfig13))]
    [XmlInclude(typeof(ValveConfig14))]
    [XmlInclude(typeof(ValveConfig15))]
    [XmlInclude(typeof(AuxGPIODir))]
    [XmlInclude(typeof(AuxGPIOState))]
    [XmlInclude(typeof(AuxGPIOSet))]
    [XmlInclude(typeof(AuxGPIOClear))]
    [XmlInclude(typeof(AuxGPIOInputRiseEvent))]
    [XmlInclude(typeof(AuxGPIOInputFallEvent))]
    [XmlInclude(typeof(AuxGPIORisingInputs))]
    [XmlInclude(typeof(AuxGPIOFallingInputs))]
    [XmlInclude(typeof(PokePin))]
    [XmlInclude(typeof(PokePinInverted))]
    [XmlInclude(typeof(PokeState))]
    [XmlInclude(typeof(RawPokeState))]
    [XmlInclude(typeof(PokeDometer))]
    [XmlInclude(typeof(FSMState))]
    [XmlInclude(typeof(ForceFSM))]
    [XmlInclude(typeof(QueuedOdorIndex))]
    [XmlInclude(typeof(VacuumCloseTimeUS))]
    [XmlInclude(typeof(MinOdorDeliveryTimeUS))]
    [XmlInclude(typeof(MaxOdorDeliveryTimeUS))]
    [XmlInclude(typeof(OdorTransitionTimeUS))]
    [XmlInclude(typeof(VacuumSetupTimeUS))]
    [XmlInclude(typeof(FinalValveEnergizedTimeUS))]
    [XmlInclude(typeof(MinimumPokeTimeUS))]
    [XmlInclude(typeof(CamPin))]
    [XmlInclude(typeof(CamPinState))]
    [XmlInclude(typeof(FrameRate))]
    [XmlInclude(typeof(DutyCycle))]
    [XmlInclude(typeof(EnableCamTrigger))]
    [XmlInclude(typeof(EnableValveLeds))]
    [Description("Formats a sequence of values as specific DelphiController register messages.")]
    public partial class Format : FormatBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class.
        /// </summary>
        public Format()
        {
            Register = new ValveState();
        }

        string INamedElement.Name => $"{nameof(DelphiController)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents a register that set the enabled/disabled state (enabled = 1) of all valves.
    /// </summary>
    [Description("Set the enabled/disabled state (enabled = 1) of all valves")]
    public partial class ValveState
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveState"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="ValveState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ValveState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ValveMask GetPayload(HarpMessage message)
        {
            return (ValveMask)message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ValveMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadUInt16();
            return Timestamped.Create((ValveMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ValveMask value)
        {
            return HarpMessage.FromUInt16(Address, messageType, (ushort)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ValveMask value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveState register.
    /// </summary>
    /// <seealso cref="ValveState"/>
    [Description("Filters and selects timestamped messages from the ValveState register.")]
    public partial class TimestampedValveState
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveState"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ValveMask> GetPayload(HarpMessage message)
        {
            return ValveState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that write a 1 to any bit to enable the corresponding valve.
    /// </summary>
    [Description("Write a 1 to any bit to enable the corresponding valve.")]
    public partial class ValvesSet
    {
        /// <summary>
        /// Represents the address of the <see cref="ValvesSet"/> register. This field is constant.
        /// </summary>
        public const int Address = 33;

        /// <summary>
        /// Represents the payload type of the <see cref="ValvesSet"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="ValvesSet"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ValvesSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ValveMask GetPayload(HarpMessage message)
        {
            return (ValveMask)message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValvesSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ValveMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadUInt16();
            return Timestamped.Create((ValveMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValvesSet"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValvesSet"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ValveMask value)
        {
            return HarpMessage.FromUInt16(Address, messageType, (ushort)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValvesSet"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValvesSet"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ValveMask value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValvesSet register.
    /// </summary>
    /// <seealso cref="ValvesSet"/>
    [Description("Filters and selects timestamped messages from the ValvesSet register.")]
    public partial class TimestampedValvesSet
    {
        /// <summary>
        /// Represents the address of the <see cref="ValvesSet"/> register. This field is constant.
        /// </summary>
        public const int Address = ValvesSet.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValvesSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ValveMask> GetPayload(HarpMessage message)
        {
            return ValvesSet.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that write a 1 to any bit to disable the corresponding valve.
    /// </summary>
    [Description("Write a 1 to any bit to disable the corresponding valve.")]
    public partial class ValvesClear
    {
        /// <summary>
        /// Represents the address of the <see cref="ValvesClear"/> register. This field is constant.
        /// </summary>
        public const int Address = 34;

        /// <summary>
        /// Represents the payload type of the <see cref="ValvesClear"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="ValvesClear"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ValvesClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ValveMask GetPayload(HarpMessage message)
        {
            return (ValveMask)message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValvesClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ValveMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadUInt16();
            return Timestamped.Create((ValveMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValvesClear"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValvesClear"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ValveMask value)
        {
            return HarpMessage.FromUInt16(Address, messageType, (ushort)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValvesClear"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValvesClear"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ValveMask value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValvesClear register.
    /// </summary>
    /// <seealso cref="ValvesClear"/>
    [Description("Filters and selects timestamped messages from the ValvesClear register.")]
    public partial class TimestampedValvesClear
    {
        /// <summary>
        /// Represents the address of the <see cref="ValvesClear"/> register. This field is constant.
        /// </summary>
        public const int Address = ValvesClear.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValvesClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ValveMask> GetPayload(HarpMessage message)
        {
            return ValvesClear.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.")]
    public partial class ValveConfig0
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig0"/> register. This field is constant.
        /// </summary>
        public const int Address = 35;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig0"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig0"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig0"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig0"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig0"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig0"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig0 register.
    /// </summary>
    /// <seealso cref="ValveConfig0"/>
    [Description("Filters and selects timestamped messages from the ValveConfig0 register.")]
    public partial class TimestampedValveConfig0
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig0"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig0.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig0.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.")]
    public partial class ValveConfig1
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig1"/> register. This field is constant.
        /// </summary>
        public const int Address = 36;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig1"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig1"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig1"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig1"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig1"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig1"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig1 register.
    /// </summary>
    /// <seealso cref="ValveConfig1"/>
    [Description("Filters and selects timestamped messages from the ValveConfig1 register.")]
    public partial class TimestampedValveConfig1
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig1"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig1.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig1.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.")]
    public partial class ValveConfig2
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig2"/> register. This field is constant.
        /// </summary>
        public const int Address = 37;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig2"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig2"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig2"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig2"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig2"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig2"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig2"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig2"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig2 register.
    /// </summary>
    /// <seealso cref="ValveConfig2"/>
    [Description("Filters and selects timestamped messages from the ValveConfig2 register.")]
    public partial class TimestampedValveConfig2
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig2"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig2.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig2"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig2.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.")]
    public partial class ValveConfig3
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig3"/> register. This field is constant.
        /// </summary>
        public const int Address = 38;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig3"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig3"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig3"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig3"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig3"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig3"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig3"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig3"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig3 register.
    /// </summary>
    /// <seealso cref="ValveConfig3"/>
    [Description("Filters and selects timestamped messages from the ValveConfig3 register.")]
    public partial class TimestampedValveConfig3
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig3"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig3.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig3"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig3.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.")]
    public partial class ValveConfig4
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig4"/> register. This field is constant.
        /// </summary>
        public const int Address = 39;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig4"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig4"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig4"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig4"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig4"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig4"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig4"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig4"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig4 register.
    /// </summary>
    /// <seealso cref="ValveConfig4"/>
    [Description("Filters and selects timestamped messages from the ValveConfig4 register.")]
    public partial class TimestampedValveConfig4
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig4"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig4.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig4"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig4.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.")]
    public partial class ValveConfig5
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig5"/> register. This field is constant.
        /// </summary>
        public const int Address = 40;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig5"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig5"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig5"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig5"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig5"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig5"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig5"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig5"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig5 register.
    /// </summary>
    /// <seealso cref="ValveConfig5"/>
    [Description("Filters and selects timestamped messages from the ValveConfig5 register.")]
    public partial class TimestampedValveConfig5
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig5"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig5.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig5"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig5.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.")]
    public partial class ValveConfig6
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig6"/> register. This field is constant.
        /// </summary>
        public const int Address = 41;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig6"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig6"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig6"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig6"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig6"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig6"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig6"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig6"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig6 register.
    /// </summary>
    /// <seealso cref="ValveConfig6"/>
    [Description("Filters and selects timestamped messages from the ValveConfig6 register.")]
    public partial class TimestampedValveConfig6
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig6"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig6.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig6"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig6.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.")]
    public partial class ValveConfig7
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig7"/> register. This field is constant.
        /// </summary>
        public const int Address = 42;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig7"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig7"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig7"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig7"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig7"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig7"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig7"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig7"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig7 register.
    /// </summary>
    /// <seealso cref="ValveConfig7"/>
    [Description("Filters and selects timestamped messages from the ValveConfig7 register.")]
    public partial class TimestampedValveConfig7
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig7"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig7.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig7"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig7.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.")]
    public partial class ValveConfig8
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig8"/> register. This field is constant.
        /// </summary>
        public const int Address = 43;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig8"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig8"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig8"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig8"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig8"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig8"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig8"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig8"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig8 register.
    /// </summary>
    /// <seealso cref="ValveConfig8"/>
    [Description("Filters and selects timestamped messages from the ValveConfig8 register.")]
    public partial class TimestampedValveConfig8
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig8"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig8.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig8"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig8.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.")]
    public partial class ValveConfig9
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig9"/> register. This field is constant.
        /// </summary>
        public const int Address = 44;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig9"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig9"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig9"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig9"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig9"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig9"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig9"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig9"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig9 register.
    /// </summary>
    /// <seealso cref="ValveConfig9"/>
    [Description("Filters and selects timestamped messages from the ValveConfig9 register.")]
    public partial class TimestampedValveConfig9
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig9"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig9.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig9"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig9.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.")]
    public partial class ValveConfig10
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig10"/> register. This field is constant.
        /// </summary>
        public const int Address = 45;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig10"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig10"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig10"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig10"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig10"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig10"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig10"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig10"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig10 register.
    /// </summary>
    /// <seealso cref="ValveConfig10"/>
    [Description("Filters and selects timestamped messages from the ValveConfig10 register.")]
    public partial class TimestampedValveConfig10
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig10"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig10.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig10"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig10.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.")]
    public partial class ValveConfig11
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig11"/> register. This field is constant.
        /// </summary>
        public const int Address = 46;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig11"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig11"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig11"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig11"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig11"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig11"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig11"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig11"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig11 register.
    /// </summary>
    /// <seealso cref="ValveConfig11"/>
    [Description("Filters and selects timestamped messages from the ValveConfig11 register.")]
    public partial class TimestampedValveConfig11
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig11"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig11.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig11"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig11.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.")]
    public partial class ValveConfig12
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig12"/> register. This field is constant.
        /// </summary>
        public const int Address = 47;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig12"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig12"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig12"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig12"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig12"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig12"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig12"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig12"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig12 register.
    /// </summary>
    /// <seealso cref="ValveConfig12"/>
    [Description("Filters and selects timestamped messages from the ValveConfig12 register.")]
    public partial class TimestampedValveConfig12
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig12"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig12.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig12"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig12.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.")]
    public partial class ValveConfig13
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig13"/> register. This field is constant.
        /// </summary>
        public const int Address = 48;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig13"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig13"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig13"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig13"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig13"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig13"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig13"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig13"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig13 register.
    /// </summary>
    /// <seealso cref="ValveConfig13"/>
    [Description("Filters and selects timestamped messages from the ValveConfig13 register.")]
    public partial class TimestampedValveConfig13
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig13"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig13.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig13"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig13.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.")]
    public partial class ValveConfig14
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig14"/> register. This field is constant.
        /// </summary>
        public const int Address = 49;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig14"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig14"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig14"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig14"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig14"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig14"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig14"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig14"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig14 register.
    /// </summary>
    /// <seealso cref="ValveConfig14"/>
    [Description("Filters and selects timestamped messages from the ValveConfig14 register.")]
    public partial class TimestampedValveConfig14
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig14"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig14.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig14"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig14.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.
    /// </summary>
    [Description("the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.")]
    public partial class ValveConfig15
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig15"/> register. This field is constant.
        /// </summary>
        public const int Address = 50;

        /// <summary>
        /// Represents the payload type of the <see cref="ValveConfig15"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ValveConfig15"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 12;

        /// <summary>
        /// Returns the payload data for <see cref="ValveConfig15"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveConfig15"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ValveConfig15"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig15"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ValveConfig15"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ValveConfig15"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ValveConfig15 register.
    /// </summary>
    /// <seealso cref="ValveConfig15"/>
    [Description("Filters and selects timestamped messages from the ValveConfig15 register.")]
    public partial class TimestampedValveConfig15
    {
        /// <summary>
        /// Represents the address of the <see cref="ValveConfig15"/> register. This field is constant.
        /// </summary>
        public const int Address = ValveConfig15.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ValveConfig15"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return ValveConfig15.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specify each auxiliary GPIO pin as an input (0) or output (1).
    /// </summary>
    [Description("Specify each auxiliary GPIO pin as an input (0) or output (1).")]
    public partial class AuxGPIODir
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIODir"/> register. This field is constant.
        /// </summary>
        public const int Address = 51;

        /// <summary>
        /// Represents the payload type of the <see cref="AuxGPIODir"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="AuxGPIODir"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="AuxGPIODir"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static AuxGPIOMask GetPayload(HarpMessage message)
        {
            return (AuxGPIOMask)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="AuxGPIODir"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((AuxGPIOMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="AuxGPIODir"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIODir"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="AuxGPIODir"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIODir"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// AuxGPIODir register.
    /// </summary>
    /// <seealso cref="AuxGPIODir"/>
    [Description("Filters and selects timestamped messages from the AuxGPIODir register.")]
    public partial class TimestampedAuxGPIODir
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIODir"/> register. This field is constant.
        /// </summary>
        public const int Address = AuxGPIODir.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="AuxGPIODir"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetPayload(HarpMessage message)
        {
            return AuxGPIODir.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the state (one or off) of any auxiliary GPIO pins specified as outputs.
    /// </summary>
    [Description("Set the state (one or off) of any auxiliary GPIO pins specified as outputs.")]
    public partial class AuxGPIOState
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOState"/> register. This field is constant.
        /// </summary>
        public const int Address = 52;

        /// <summary>
        /// Represents the payload type of the <see cref="AuxGPIOState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="AuxGPIOState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="AuxGPIOState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static AuxGPIOMask GetPayload(HarpMessage message)
        {
            return (AuxGPIOMask)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="AuxGPIOState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((AuxGPIOMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="AuxGPIOState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="AuxGPIOState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// AuxGPIOState register.
    /// </summary>
    /// <seealso cref="AuxGPIOState"/>
    [Description("Filters and selects timestamped messages from the AuxGPIOState register.")]
    public partial class TimestampedAuxGPIOState
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOState"/> register. This field is constant.
        /// </summary>
        public const int Address = AuxGPIOState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="AuxGPIOState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetPayload(HarpMessage message)
        {
            return AuxGPIOState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that when writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.
    /// </summary>
    [Description("When writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.")]
    public partial class AuxGPIOSet
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOSet"/> register. This field is constant.
        /// </summary>
        public const int Address = 53;

        /// <summary>
        /// Represents the payload type of the <see cref="AuxGPIOSet"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="AuxGPIOSet"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="AuxGPIOSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static AuxGPIOMask GetPayload(HarpMessage message)
        {
            return (AuxGPIOMask)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="AuxGPIOSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((AuxGPIOMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="AuxGPIOSet"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOSet"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="AuxGPIOSet"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOSet"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// AuxGPIOSet register.
    /// </summary>
    /// <seealso cref="AuxGPIOSet"/>
    [Description("Filters and selects timestamped messages from the AuxGPIOSet register.")]
    public partial class TimestampedAuxGPIOSet
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOSet"/> register. This field is constant.
        /// </summary>
        public const int Address = AuxGPIOSet.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="AuxGPIOSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetPayload(HarpMessage message)
        {
            return AuxGPIOSet.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that when writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.
    /// </summary>
    [Description("When writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.")]
    public partial class AuxGPIOClear
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOClear"/> register. This field is constant.
        /// </summary>
        public const int Address = 54;

        /// <summary>
        /// Represents the payload type of the <see cref="AuxGPIOClear"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="AuxGPIOClear"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="AuxGPIOClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static AuxGPIOMask GetPayload(HarpMessage message)
        {
            return (AuxGPIOMask)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="AuxGPIOClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((AuxGPIOMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="AuxGPIOClear"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOClear"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="AuxGPIOClear"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOClear"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// AuxGPIOClear register.
    /// </summary>
    /// <seealso cref="AuxGPIOClear"/>
    [Description("Filters and selects timestamped messages from the AuxGPIOClear register.")]
    public partial class TimestampedAuxGPIOClear
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOClear"/> register. This field is constant.
        /// </summary>
        public const int Address = AuxGPIOClear.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="AuxGPIOClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetPayload(HarpMessage message)
        {
            return AuxGPIOClear.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that manipulates messages from register AuxGPIOInputRiseEvent.
    /// </summary>
    [Description("")]
    public partial class AuxGPIOInputRiseEvent
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOInputRiseEvent"/> register. This field is constant.
        /// </summary>
        public const int Address = 55;

        /// <summary>
        /// Represents the payload type of the <see cref="AuxGPIOInputRiseEvent"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="AuxGPIOInputRiseEvent"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="AuxGPIOInputRiseEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static AuxGPIOMask GetPayload(HarpMessage message)
        {
            return (AuxGPIOMask)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="AuxGPIOInputRiseEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((AuxGPIOMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="AuxGPIOInputRiseEvent"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOInputRiseEvent"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="AuxGPIOInputRiseEvent"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOInputRiseEvent"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// AuxGPIOInputRiseEvent register.
    /// </summary>
    /// <seealso cref="AuxGPIOInputRiseEvent"/>
    [Description("Filters and selects timestamped messages from the AuxGPIOInputRiseEvent register.")]
    public partial class TimestampedAuxGPIOInputRiseEvent
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOInputRiseEvent"/> register. This field is constant.
        /// </summary>
        public const int Address = AuxGPIOInputRiseEvent.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="AuxGPIOInputRiseEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetPayload(HarpMessage message)
        {
            return AuxGPIOInputRiseEvent.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that manipulates messages from register AuxGPIOInputFallEvent.
    /// </summary>
    [Description("")]
    public partial class AuxGPIOInputFallEvent
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOInputFallEvent"/> register. This field is constant.
        /// </summary>
        public const int Address = 56;

        /// <summary>
        /// Represents the payload type of the <see cref="AuxGPIOInputFallEvent"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="AuxGPIOInputFallEvent"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="AuxGPIOInputFallEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static AuxGPIOMask GetPayload(HarpMessage message)
        {
            return (AuxGPIOMask)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="AuxGPIOInputFallEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((AuxGPIOMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="AuxGPIOInputFallEvent"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOInputFallEvent"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="AuxGPIOInputFallEvent"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOInputFallEvent"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// AuxGPIOInputFallEvent register.
    /// </summary>
    /// <seealso cref="AuxGPIOInputFallEvent"/>
    [Description("Filters and selects timestamped messages from the AuxGPIOInputFallEvent register.")]
    public partial class TimestampedAuxGPIOInputFallEvent
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOInputFallEvent"/> register. This field is constant.
        /// </summary>
        public const int Address = AuxGPIOInputFallEvent.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="AuxGPIOInputFallEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetPayload(HarpMessage message)
        {
            return AuxGPIOInputFallEvent.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that manipulates messages from register AuxGPIORisingInputs.
    /// </summary>
    [Description("")]
    public partial class AuxGPIORisingInputs
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIORisingInputs"/> register. This field is constant.
        /// </summary>
        public const int Address = 57;

        /// <summary>
        /// Represents the payload type of the <see cref="AuxGPIORisingInputs"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="AuxGPIORisingInputs"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="AuxGPIORisingInputs"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static AuxGPIOMask GetPayload(HarpMessage message)
        {
            return (AuxGPIOMask)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="AuxGPIORisingInputs"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((AuxGPIOMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="AuxGPIORisingInputs"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIORisingInputs"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="AuxGPIORisingInputs"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIORisingInputs"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// AuxGPIORisingInputs register.
    /// </summary>
    /// <seealso cref="AuxGPIORisingInputs"/>
    [Description("Filters and selects timestamped messages from the AuxGPIORisingInputs register.")]
    public partial class TimestampedAuxGPIORisingInputs
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIORisingInputs"/> register. This field is constant.
        /// </summary>
        public const int Address = AuxGPIORisingInputs.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="AuxGPIORisingInputs"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetPayload(HarpMessage message)
        {
            return AuxGPIORisingInputs.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that manipulates messages from register AuxGPIOFallingInputs.
    /// </summary>
    [Description("")]
    public partial class AuxGPIOFallingInputs
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOFallingInputs"/> register. This field is constant.
        /// </summary>
        public const int Address = 58;

        /// <summary>
        /// Represents the payload type of the <see cref="AuxGPIOFallingInputs"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="AuxGPIOFallingInputs"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="AuxGPIOFallingInputs"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static AuxGPIOMask GetPayload(HarpMessage message)
        {
            return (AuxGPIOMask)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="AuxGPIOFallingInputs"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((AuxGPIOMask)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="AuxGPIOFallingInputs"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOFallingInputs"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="AuxGPIOFallingInputs"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AuxGPIOFallingInputs"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, AuxGPIOMask value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// AuxGPIOFallingInputs register.
    /// </summary>
    /// <seealso cref="AuxGPIOFallingInputs"/>
    [Description("Filters and selects timestamped messages from the AuxGPIOFallingInputs register.")]
    public partial class TimestampedAuxGPIOFallingInputs
    {
        /// <summary>
        /// Represents the address of the <see cref="AuxGPIOFallingInputs"/> register. This field is constant.
        /// </summary>
        public const int Address = AuxGPIOFallingInputs.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="AuxGPIOFallingInputs"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<AuxGPIOMask> GetPayload(HarpMessage message)
        {
            return AuxGPIOFallingInputs.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that which poke ports are active.
    /// </summary>
    [Description("which poke ports are active.")]
    public partial class PokePin
    {
        /// <summary>
        /// Represents the address of the <see cref="PokePin"/> register. This field is constant.
        /// </summary>
        public const int Address = 59;

        /// <summary>
        /// Represents the payload type of the <see cref="PokePin"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PokePin"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PokePin"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PokePin"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PokePin"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PokePin"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PokePin"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PokePin"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PokePin register.
    /// </summary>
    /// <seealso cref="PokePin"/>
    [Description("Filters and selects timestamped messages from the PokePin register.")]
    public partial class TimestampedPokePin
    {
        /// <summary>
        /// Represents the address of the <see cref="PokePin"/> register. This field is constant.
        /// </summary>
        public const int Address = PokePin.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PokePin"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return PokePin.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).
    /// </summary>
    [Description("Which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).")]
    public partial class PokePinInverted
    {
        /// <summary>
        /// Represents the address of the <see cref="PokePinInverted"/> register. This field is constant.
        /// </summary>
        public const int Address = 60;

        /// <summary>
        /// Represents the payload type of the <see cref="PokePinInverted"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PokePinInverted"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PokePinInverted"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PokePinInverted"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PokePinInverted"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PokePinInverted"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PokePinInverted"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PokePinInverted"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PokePinInverted register.
    /// </summary>
    /// <seealso cref="PokePinInverted"/>
    [Description("Filters and selects timestamped messages from the PokePinInverted register.")]
    public partial class TimestampedPokePinInverted
    {
        /// <summary>
        /// Represents the address of the <see cref="PokePinInverted"/> register. This field is constant.
        /// </summary>
        public const int Address = PokePinInverted.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PokePinInverted"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return PokePinInverted.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.
    /// </summary>
    [Description("The state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.")]
    public partial class PokeState
    {
        /// <summary>
        /// Represents the address of the <see cref="PokeState"/> register. This field is constant.
        /// </summary>
        public const int Address = 61;

        /// <summary>
        /// Represents the payload type of the <see cref="PokeState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PokeState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PokeState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PokeState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PokeState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PokeState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PokeState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PokeState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PokeState register.
    /// </summary>
    /// <seealso cref="PokeState"/>
    [Description("Filters and selects timestamped messages from the PokeState register.")]
    public partial class TimestampedPokeState
    {
        /// <summary>
        /// Represents the address of the <see cref="PokeState"/> register. This field is constant.
        /// </summary>
        public const int Address = PokeState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PokeState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return PokeState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).
    /// </summary>
    [Description("The raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).")]
    public partial class RawPokeState
    {
        /// <summary>
        /// Represents the address of the <see cref="RawPokeState"/> register. This field is constant.
        /// </summary>
        public const int Address = 62;

        /// <summary>
        /// Represents the payload type of the <see cref="RawPokeState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="RawPokeState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="RawPokeState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="RawPokeState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="RawPokeState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RawPokeState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="RawPokeState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RawPokeState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// RawPokeState register.
    /// </summary>
    /// <seealso cref="RawPokeState"/>
    [Description("Filters and selects timestamped messages from the RawPokeState register.")]
    public partial class TimestampedRawPokeState
    {
        /// <summary>
        /// Represents the address of the <see cref="RawPokeState"/> register. This field is constant.
        /// </summary>
        public const int Address = RawPokeState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="RawPokeState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return RawPokeState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that number of mouse pokes per port since boot or reset.
    /// </summary>
    [Description("number of mouse pokes per port since boot or reset.")]
    public partial class PokeDometer
    {
        /// <summary>
        /// Represents the address of the <see cref="PokeDometer"/> register. This field is constant.
        /// </summary>
        public const int Address = 63;

        /// <summary>
        /// Represents the payload type of the <see cref="PokeDometer"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="PokeDometer"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PokeDometer"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PokeDometer"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PokeDometer"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PokeDometer"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PokeDometer"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PokeDometer"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PokeDometer register.
    /// </summary>
    /// <seealso cref="PokeDometer"/>
    [Description("Filters and selects timestamped messages from the PokeDometer register.")]
    public partial class TimestampedPokeDometer
    {
        /// <summary>
        /// Represents the address of the <see cref="PokeDometer"/> register. This field is constant.
        /// </summary>
        public const int Address = PokeDometer.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PokeDometer"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return PokeDometer.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
    /// </summary>
    [Description("Enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.")]
    public partial class FSMState
    {
        /// <summary>
        /// Represents the address of the <see cref="FSMState"/> register. This field is constant.
        /// </summary>
        public const int Address = 64;

        /// <summary>
        /// Represents the payload type of the <see cref="FSMState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="FSMState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="FSMState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="FSMState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="FSMState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FSMState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="FSMState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FSMState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// FSMState register.
    /// </summary>
    /// <seealso cref="FSMState"/>
    [Description("Filters and selects timestamped messages from the FSMState register.")]
    public partial class TimestampedFSMState
    {
        /// <summary>
        /// Represents the address of the <see cref="FSMState"/> register. This field is constant.
        /// </summary>
        public const int Address = FSMState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="FSMState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return FSMState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.
    /// </summary>
    [Description("Force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.")]
    public partial class ForceFSM
    {
        /// <summary>
        /// Represents the address of the <see cref="ForceFSM"/> register. This field is constant.
        /// </summary>
        public const int Address = 65;

        /// <summary>
        /// Represents the payload type of the <see cref="ForceFSM"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ForceFSM"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ForceFSM"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ForceFSM"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ForceFSM"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ForceFSM"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ForceFSM"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ForceFSM"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ForceFSM register.
    /// </summary>
    /// <seealso cref="ForceFSM"/>
    [Description("Filters and selects timestamped messages from the ForceFSM register.")]
    public partial class TimestampedForceFSM
    {
        /// <summary>
        /// Represents the address of the <see cref="ForceFSM"/> register. This field is constant.
        /// </summary>
        public const int Address = ForceFSM.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ForceFSM"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return ForceFSM.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed.
    /// </summary>
    [Description("Queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed")]
    public partial class QueuedOdorIndex
    {
        /// <summary>
        /// Represents the address of the <see cref="QueuedOdorIndex"/> register. This field is constant.
        /// </summary>
        public const int Address = 66;

        /// <summary>
        /// Represents the payload type of the <see cref="QueuedOdorIndex"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.S8;

        /// <summary>
        /// Represents the length of the <see cref="QueuedOdorIndex"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="QueuedOdorIndex"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static sbyte GetPayload(HarpMessage message)
        {
            return message.GetPayloadSByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="QueuedOdorIndex"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<sbyte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="QueuedOdorIndex"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="QueuedOdorIndex"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, sbyte value)
        {
            return HarpMessage.FromSByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="QueuedOdorIndex"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="QueuedOdorIndex"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, sbyte value)
        {
            return HarpMessage.FromSByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// QueuedOdorIndex register.
    /// </summary>
    /// <seealso cref="QueuedOdorIndex"/>
    [Description("Filters and selects timestamped messages from the QueuedOdorIndex register.")]
    public partial class TimestampedQueuedOdorIndex
    {
        /// <summary>
        /// Represents the address of the <see cref="QueuedOdorIndex"/> register. This field is constant.
        /// </summary>
        public const int Address = QueuedOdorIndex.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="QueuedOdorIndex"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<sbyte> GetPayload(HarpMessage message)
        {
            return QueuedOdorIndex.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time alotted (in microseconds) for the vacuum valve to close.
    /// </summary>
    [Description("Time alotted (in microseconds) for the vacuum valve to close.")]
    public partial class VacuumCloseTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="VacuumCloseTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 67;

        /// <summary>
        /// Represents the payload type of the <see cref="VacuumCloseTimeUS"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="VacuumCloseTimeUS"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="VacuumCloseTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="VacuumCloseTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="VacuumCloseTimeUS"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="VacuumCloseTimeUS"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="VacuumCloseTimeUS"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="VacuumCloseTimeUS"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// VacuumCloseTimeUS register.
    /// </summary>
    /// <seealso cref="VacuumCloseTimeUS"/>
    [Description("Filters and selects timestamped messages from the VacuumCloseTimeUS register.")]
    public partial class TimestampedVacuumCloseTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="VacuumCloseTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = VacuumCloseTimeUS.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="VacuumCloseTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return VacuumCloseTimeUS.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that minimum time alotted (in microseconds) for the odor delivery state.
    /// </summary>
    [Description("Minimum time alotted (in microseconds) for the odor delivery state.")]
    public partial class MinOdorDeliveryTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="MinOdorDeliveryTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 68;

        /// <summary>
        /// Represents the payload type of the <see cref="MinOdorDeliveryTimeUS"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="MinOdorDeliveryTimeUS"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="MinOdorDeliveryTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="MinOdorDeliveryTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="MinOdorDeliveryTimeUS"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="MinOdorDeliveryTimeUS"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="MinOdorDeliveryTimeUS"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="MinOdorDeliveryTimeUS"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// MinOdorDeliveryTimeUS register.
    /// </summary>
    /// <seealso cref="MinOdorDeliveryTimeUS"/>
    [Description("Filters and selects timestamped messages from the MinOdorDeliveryTimeUS register.")]
    public partial class TimestampedMinOdorDeliveryTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="MinOdorDeliveryTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = MinOdorDeliveryTimeUS.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="MinOdorDeliveryTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return MinOdorDeliveryTimeUS.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that maximum time alotted (in microseconds) for the odor delivery state.
    /// </summary>
    [Description("Maximum time alotted (in microseconds) for the odor delivery state.")]
    public partial class MaxOdorDeliveryTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="MaxOdorDeliveryTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 69;

        /// <summary>
        /// Represents the payload type of the <see cref="MaxOdorDeliveryTimeUS"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="MaxOdorDeliveryTimeUS"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="MaxOdorDeliveryTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="MaxOdorDeliveryTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="MaxOdorDeliveryTimeUS"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="MaxOdorDeliveryTimeUS"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="MaxOdorDeliveryTimeUS"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="MaxOdorDeliveryTimeUS"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// MaxOdorDeliveryTimeUS register.
    /// </summary>
    /// <seealso cref="MaxOdorDeliveryTimeUS"/>
    [Description("Filters and selects timestamped messages from the MaxOdorDeliveryTimeUS register.")]
    public partial class TimestampedMaxOdorDeliveryTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="MaxOdorDeliveryTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = MaxOdorDeliveryTimeUS.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="MaxOdorDeliveryTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return MaxOdorDeliveryTimeUS.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time alotted (in microseconds) before the vacuum turns on to remove the current odor.
    /// </summary>
    [Description("Time alotted (in microseconds) before the vacuum turns on to remove the current odor.")]
    public partial class OdorTransitionTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="OdorTransitionTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 70;

        /// <summary>
        /// Represents the payload type of the <see cref="OdorTransitionTimeUS"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="OdorTransitionTimeUS"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OdorTransitionTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OdorTransitionTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OdorTransitionTimeUS"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OdorTransitionTimeUS"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OdorTransitionTimeUS"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OdorTransitionTimeUS"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OdorTransitionTimeUS register.
    /// </summary>
    /// <seealso cref="OdorTransitionTimeUS"/>
    [Description("Filters and selects timestamped messages from the OdorTransitionTimeUS register.")]
    public partial class TimestampedOdorTransitionTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="OdorTransitionTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = OdorTransitionTimeUS.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OdorTransitionTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return OdorTransitionTimeUS.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time alotted (in microseconds) for the vacuum to open.
    /// </summary>
    [Description("Time alotted (in microseconds) for the vacuum to open.")]
    public partial class VacuumSetupTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="VacuumSetupTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 71;

        /// <summary>
        /// Represents the payload type of the <see cref="VacuumSetupTimeUS"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="VacuumSetupTimeUS"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="VacuumSetupTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="VacuumSetupTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="VacuumSetupTimeUS"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="VacuumSetupTimeUS"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="VacuumSetupTimeUS"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="VacuumSetupTimeUS"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// VacuumSetupTimeUS register.
    /// </summary>
    /// <seealso cref="VacuumSetupTimeUS"/>
    [Description("Filters and selects timestamped messages from the VacuumSetupTimeUS register.")]
    public partial class TimestampedVacuumSetupTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="VacuumSetupTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = VacuumSetupTimeUS.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="VacuumSetupTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return VacuumSetupTimeUS.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time alotted (in microseconds) for the final valve to open and remain on.
    /// </summary>
    [Description("Time alotted (in microseconds) for the final valve to open and remain on.")]
    public partial class FinalValveEnergizedTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="FinalValveEnergizedTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 72;

        /// <summary>
        /// Represents the payload type of the <see cref="FinalValveEnergizedTimeUS"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="FinalValveEnergizedTimeUS"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="FinalValveEnergizedTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="FinalValveEnergizedTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="FinalValveEnergizedTimeUS"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FinalValveEnergizedTimeUS"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="FinalValveEnergizedTimeUS"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FinalValveEnergizedTimeUS"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// FinalValveEnergizedTimeUS register.
    /// </summary>
    /// <seealso cref="FinalValveEnergizedTimeUS"/>
    [Description("Filters and selects timestamped messages from the FinalValveEnergizedTimeUS register.")]
    public partial class TimestampedFinalValveEnergizedTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="FinalValveEnergizedTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = FinalValveEnergizedTimeUS.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="FinalValveEnergizedTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return FinalValveEnergizedTimeUS.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.
    /// </summary>
    [Description("Minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.")]
    public partial class MinimumPokeTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="MinimumPokeTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 73;

        /// <summary>
        /// Represents the payload type of the <see cref="MinimumPokeTimeUS"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="MinimumPokeTimeUS"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="MinimumPokeTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="MinimumPokeTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="MinimumPokeTimeUS"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="MinimumPokeTimeUS"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="MinimumPokeTimeUS"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="MinimumPokeTimeUS"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// MinimumPokeTimeUS register.
    /// </summary>
    /// <seealso cref="MinimumPokeTimeUS"/>
    [Description("Filters and selects timestamped messages from the MinimumPokeTimeUS register.")]
    public partial class TimestampedMinimumPokeTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="MinimumPokeTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = MinimumPokeTimeUS.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="MinimumPokeTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return MinimumPokeTimeUS.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the GPIO output pin used for camera triggering. Default pin is 26.
    /// </summary>
    [Description("The GPIO output pin used for camera triggering. Default pin is 26.")]
    public partial class CamPin
    {
        /// <summary>
        /// Represents the address of the <see cref="CamPin"/> register. This field is constant.
        /// </summary>
        public const int Address = 74;

        /// <summary>
        /// Represents the payload type of the <see cref="CamPin"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="CamPin"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="CamPin"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="CamPin"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="CamPin"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="CamPin"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="CamPin"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="CamPin"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// CamPin register.
    /// </summary>
    /// <seealso cref="CamPin"/>
    [Description("Filters and selects timestamped messages from the CamPin register.")]
    public partial class TimestampedCamPin
    {
        /// <summary>
        /// Represents the address of the <see cref="CamPin"/> register. This field is constant.
        /// </summary>
        public const int Address = CamPin.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="CamPin"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return CamPin.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
    /// </summary>
    [Description("Event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
    public partial class CamPinState
    {
        /// <summary>
        /// Represents the address of the <see cref="CamPinState"/> register. This field is constant.
        /// </summary>
        public const int Address = 75;

        /// <summary>
        /// Represents the payload type of the <see cref="CamPinState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="CamPinState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="CamPinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="CamPinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="CamPinState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="CamPinState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="CamPinState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="CamPinState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// CamPinState register.
    /// </summary>
    /// <seealso cref="CamPinState"/>
    [Description("Filters and selects timestamped messages from the CamPinState register.")]
    public partial class TimestampedCamPinState
    {
        /// <summary>
        /// Represents the address of the <see cref="CamPinState"/> register. This field is constant.
        /// </summary>
        public const int Address = CamPinState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="CamPinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return CamPinState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the frame rate of the camera trigger/ frequency of the PWM signal.
    /// </summary>
    [Description("Set the frame rate of the camera trigger/ frequency of the PWM signal.")]
    public partial class FrameRate
    {
        /// <summary>
        /// Represents the address of the <see cref="FrameRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 76;

        /// <summary>
        /// Represents the payload type of the <see cref="FrameRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="FrameRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="FrameRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="FrameRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="FrameRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FrameRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="FrameRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FrameRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// FrameRate register.
    /// </summary>
    /// <seealso cref="FrameRate"/>
    [Description("Filters and selects timestamped messages from the FrameRate register.")]
    public partial class TimestampedFrameRate
    {
        /// <summary>
        /// Represents the address of the <see cref="FrameRate"/> register. This field is constant.
        /// </summary>
        public const int Address = FrameRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="FrameRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return FrameRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
    /// </summary>
    [Description("Set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
    public partial class DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = 77;

        /// <summary>
        /// Represents the payload type of the <see cref="DutyCycle"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="DutyCycle"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DutyCycle"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="DutyCycle"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DutyCycle"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// DutyCycle register.
    /// </summary>
    /// <seealso cref="DutyCycle"/>
    [Description("Filters and selects timestamped messages from the DutyCycle register.")]
    public partial class TimestampedDutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = DutyCycle.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return DutyCycle.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable (1) and disable (0) camera triggering/ the PWM signal.
    /// </summary>
    [Description("Enable (1) and disable (0) camera triggering/ the PWM signal.")]
    public partial class EnableCamTrigger
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableCamTrigger"/> register. This field is constant.
        /// </summary>
        public const int Address = 78;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableCamTrigger"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableCamTrigger"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableCamTrigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableCamTrigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableCamTrigger"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableCamTrigger"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableCamTrigger"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableCamTrigger"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableCamTrigger register.
    /// </summary>
    /// <seealso cref="EnableCamTrigger"/>
    [Description("Filters and selects timestamped messages from the EnableCamTrigger register.")]
    public partial class TimestampedEnableCamTrigger
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableCamTrigger"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableCamTrigger.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableCamTrigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return EnableCamTrigger.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable (1) and disable (0) valve LEDs.
    /// </summary>
    [Description("Enable (1) and disable (0) valve LEDs.")]
    public partial class EnableValveLeds
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableValveLeds"/> register. This field is constant.
        /// </summary>
        public const int Address = 79;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableValveLeds"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableValveLeds"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableValveLeds"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableValveLeds"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableValveLeds"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableValveLeds"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableValveLeds"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableValveLeds"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableValveLeds register.
    /// </summary>
    /// <seealso cref="EnableValveLeds"/>
    [Description("Filters and selects timestamped messages from the EnableValveLeds register.")]
    public partial class TimestampedEnableValveLeds
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableValveLeds"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableValveLeds.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableValveLeds"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return EnableValveLeds.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator which creates standard message payloads for the
    /// DelphiController device.
    /// </summary>
    /// <seealso cref="CreateValveStatePayload"/>
    /// <seealso cref="CreateValvesSetPayload"/>
    /// <seealso cref="CreateValvesClearPayload"/>
    /// <seealso cref="CreateValveConfig0Payload"/>
    /// <seealso cref="CreateValveConfig1Payload"/>
    /// <seealso cref="CreateValveConfig2Payload"/>
    /// <seealso cref="CreateValveConfig3Payload"/>
    /// <seealso cref="CreateValveConfig4Payload"/>
    /// <seealso cref="CreateValveConfig5Payload"/>
    /// <seealso cref="CreateValveConfig6Payload"/>
    /// <seealso cref="CreateValveConfig7Payload"/>
    /// <seealso cref="CreateValveConfig8Payload"/>
    /// <seealso cref="CreateValveConfig9Payload"/>
    /// <seealso cref="CreateValveConfig10Payload"/>
    /// <seealso cref="CreateValveConfig11Payload"/>
    /// <seealso cref="CreateValveConfig12Payload"/>
    /// <seealso cref="CreateValveConfig13Payload"/>
    /// <seealso cref="CreateValveConfig14Payload"/>
    /// <seealso cref="CreateValveConfig15Payload"/>
    /// <seealso cref="CreateAuxGPIODirPayload"/>
    /// <seealso cref="CreateAuxGPIOStatePayload"/>
    /// <seealso cref="CreateAuxGPIOSetPayload"/>
    /// <seealso cref="CreateAuxGPIOClearPayload"/>
    /// <seealso cref="CreateAuxGPIOInputRiseEventPayload"/>
    /// <seealso cref="CreateAuxGPIOInputFallEventPayload"/>
    /// <seealso cref="CreateAuxGPIORisingInputsPayload"/>
    /// <seealso cref="CreateAuxGPIOFallingInputsPayload"/>
    /// <seealso cref="CreatePokePinPayload"/>
    /// <seealso cref="CreatePokePinInvertedPayload"/>
    /// <seealso cref="CreatePokeStatePayload"/>
    /// <seealso cref="CreateRawPokeStatePayload"/>
    /// <seealso cref="CreatePokeDometerPayload"/>
    /// <seealso cref="CreateFSMStatePayload"/>
    /// <seealso cref="CreateForceFSMPayload"/>
    /// <seealso cref="CreateQueuedOdorIndexPayload"/>
    /// <seealso cref="CreateVacuumCloseTimeUSPayload"/>
    /// <seealso cref="CreateMinOdorDeliveryTimeUSPayload"/>
    /// <seealso cref="CreateMaxOdorDeliveryTimeUSPayload"/>
    /// <seealso cref="CreateOdorTransitionTimeUSPayload"/>
    /// <seealso cref="CreateVacuumSetupTimeUSPayload"/>
    /// <seealso cref="CreateFinalValveEnergizedTimeUSPayload"/>
    /// <seealso cref="CreateMinimumPokeTimeUSPayload"/>
    /// <seealso cref="CreateCamPinPayload"/>
    /// <seealso cref="CreateCamPinStatePayload"/>
    /// <seealso cref="CreateFrameRatePayload"/>
    /// <seealso cref="CreateDutyCyclePayload"/>
    /// <seealso cref="CreateEnableCamTriggerPayload"/>
    /// <seealso cref="CreateEnableValveLedsPayload"/>
    [XmlInclude(typeof(CreateValveStatePayload))]
    [XmlInclude(typeof(CreateValvesSetPayload))]
    [XmlInclude(typeof(CreateValvesClearPayload))]
    [XmlInclude(typeof(CreateValveConfig0Payload))]
    [XmlInclude(typeof(CreateValveConfig1Payload))]
    [XmlInclude(typeof(CreateValveConfig2Payload))]
    [XmlInclude(typeof(CreateValveConfig3Payload))]
    [XmlInclude(typeof(CreateValveConfig4Payload))]
    [XmlInclude(typeof(CreateValveConfig5Payload))]
    [XmlInclude(typeof(CreateValveConfig6Payload))]
    [XmlInclude(typeof(CreateValveConfig7Payload))]
    [XmlInclude(typeof(CreateValveConfig8Payload))]
    [XmlInclude(typeof(CreateValveConfig9Payload))]
    [XmlInclude(typeof(CreateValveConfig10Payload))]
    [XmlInclude(typeof(CreateValveConfig11Payload))]
    [XmlInclude(typeof(CreateValveConfig12Payload))]
    [XmlInclude(typeof(CreateValveConfig13Payload))]
    [XmlInclude(typeof(CreateValveConfig14Payload))]
    [XmlInclude(typeof(CreateValveConfig15Payload))]
    [XmlInclude(typeof(CreateAuxGPIODirPayload))]
    [XmlInclude(typeof(CreateAuxGPIOStatePayload))]
    [XmlInclude(typeof(CreateAuxGPIOSetPayload))]
    [XmlInclude(typeof(CreateAuxGPIOClearPayload))]
    [XmlInclude(typeof(CreateAuxGPIOInputRiseEventPayload))]
    [XmlInclude(typeof(CreateAuxGPIOInputFallEventPayload))]
    [XmlInclude(typeof(CreateAuxGPIORisingInputsPayload))]
    [XmlInclude(typeof(CreateAuxGPIOFallingInputsPayload))]
    [XmlInclude(typeof(CreatePokePinPayload))]
    [XmlInclude(typeof(CreatePokePinInvertedPayload))]
    [XmlInclude(typeof(CreatePokeStatePayload))]
    [XmlInclude(typeof(CreateRawPokeStatePayload))]
    [XmlInclude(typeof(CreatePokeDometerPayload))]
    [XmlInclude(typeof(CreateFSMStatePayload))]
    [XmlInclude(typeof(CreateForceFSMPayload))]
    [XmlInclude(typeof(CreateQueuedOdorIndexPayload))]
    [XmlInclude(typeof(CreateVacuumCloseTimeUSPayload))]
    [XmlInclude(typeof(CreateMinOdorDeliveryTimeUSPayload))]
    [XmlInclude(typeof(CreateMaxOdorDeliveryTimeUSPayload))]
    [XmlInclude(typeof(CreateOdorTransitionTimeUSPayload))]
    [XmlInclude(typeof(CreateVacuumSetupTimeUSPayload))]
    [XmlInclude(typeof(CreateFinalValveEnergizedTimeUSPayload))]
    [XmlInclude(typeof(CreateMinimumPokeTimeUSPayload))]
    [XmlInclude(typeof(CreateCamPinPayload))]
    [XmlInclude(typeof(CreateCamPinStatePayload))]
    [XmlInclude(typeof(CreateFrameRatePayload))]
    [XmlInclude(typeof(CreateDutyCyclePayload))]
    [XmlInclude(typeof(CreateEnableCamTriggerPayload))]
    [XmlInclude(typeof(CreateEnableValveLedsPayload))]
    [XmlInclude(typeof(CreateTimestampedValveStatePayload))]
    [XmlInclude(typeof(CreateTimestampedValvesSetPayload))]
    [XmlInclude(typeof(CreateTimestampedValvesClearPayload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig0Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig1Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig2Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig3Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig4Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig5Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig6Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig7Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig8Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig9Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig10Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig11Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig12Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig13Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig14Payload))]
    [XmlInclude(typeof(CreateTimestampedValveConfig15Payload))]
    [XmlInclude(typeof(CreateTimestampedAuxGPIODirPayload))]
    [XmlInclude(typeof(CreateTimestampedAuxGPIOStatePayload))]
    [XmlInclude(typeof(CreateTimestampedAuxGPIOSetPayload))]
    [XmlInclude(typeof(CreateTimestampedAuxGPIOClearPayload))]
    [XmlInclude(typeof(CreateTimestampedAuxGPIOInputRiseEventPayload))]
    [XmlInclude(typeof(CreateTimestampedAuxGPIOInputFallEventPayload))]
    [XmlInclude(typeof(CreateTimestampedAuxGPIORisingInputsPayload))]
    [XmlInclude(typeof(CreateTimestampedAuxGPIOFallingInputsPayload))]
    [XmlInclude(typeof(CreateTimestampedPokePinPayload))]
    [XmlInclude(typeof(CreateTimestampedPokePinInvertedPayload))]
    [XmlInclude(typeof(CreateTimestampedPokeStatePayload))]
    [XmlInclude(typeof(CreateTimestampedRawPokeStatePayload))]
    [XmlInclude(typeof(CreateTimestampedPokeDometerPayload))]
    [XmlInclude(typeof(CreateTimestampedFSMStatePayload))]
    [XmlInclude(typeof(CreateTimestampedForceFSMPayload))]
    [XmlInclude(typeof(CreateTimestampedQueuedOdorIndexPayload))]
    [XmlInclude(typeof(CreateTimestampedVacuumCloseTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedMinOdorDeliveryTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedMaxOdorDeliveryTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedOdorTransitionTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedVacuumSetupTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedFinalValveEnergizedTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedMinimumPokeTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedCamPinPayload))]
    [XmlInclude(typeof(CreateTimestampedCamPinStatePayload))]
    [XmlInclude(typeof(CreateTimestampedFrameRatePayload))]
    [XmlInclude(typeof(CreateTimestampedDutyCyclePayload))]
    [XmlInclude(typeof(CreateTimestampedEnableCamTriggerPayload))]
    [XmlInclude(typeof(CreateTimestampedEnableValveLedsPayload))]
    [Description("Creates standard message payloads for the DelphiController device.")]
    public partial class CreateMessage : CreateMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessage"/> class.
        /// </summary>
        public CreateMessage()
        {
            Payload = new CreateValveStatePayload();
        }

        string INamedElement.Name => $"{nameof(DelphiController)}.{GetElementDisplayName(Payload)}";
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the enabled/disabled state (enabled = 1) of all valves.
    /// </summary>
    [DisplayName("ValveStatePayload")]
    [Description("Creates a message payload that set the enabled/disabled state (enabled = 1) of all valves.")]
    public partial class CreateValveStatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the enabled/disabled state (enabled = 1) of all valves.
        /// </summary>
        [Description("The value that set the enabled/disabled state (enabled = 1) of all valves.")]
        public ValveMask ValveState { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ValveMask GetPayload()
        {
            return ValveState;
        }

        /// <summary>
        /// Creates a message that set the enabled/disabled state (enabled = 1) of all valves.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the enabled/disabled state (enabled = 1) of all valves.
    /// </summary>
    [DisplayName("TimestampedValveStatePayload")]
    [Description("Creates a timestamped message payload that set the enabled/disabled state (enabled = 1) of all valves.")]
    public partial class CreateTimestampedValveStatePayload : CreateValveStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the enabled/disabled state (enabled = 1) of all valves.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that write a 1 to any bit to enable the corresponding valve.
    /// </summary>
    [DisplayName("ValvesSetPayload")]
    [Description("Creates a message payload that write a 1 to any bit to enable the corresponding valve.")]
    public partial class CreateValvesSetPayload
    {
        /// <summary>
        /// Gets or sets the value that write a 1 to any bit to enable the corresponding valve.
        /// </summary>
        [Description("The value that write a 1 to any bit to enable the corresponding valve.")]
        public ValveMask ValvesSet { get; set; }

        /// <summary>
        /// Creates a message payload for the ValvesSet register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ValveMask GetPayload()
        {
            return ValvesSet;
        }

        /// <summary>
        /// Creates a message that write a 1 to any bit to enable the corresponding valve.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValvesSet register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValvesSet.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that write a 1 to any bit to enable the corresponding valve.
    /// </summary>
    [DisplayName("TimestampedValvesSetPayload")]
    [Description("Creates a timestamped message payload that write a 1 to any bit to enable the corresponding valve.")]
    public partial class CreateTimestampedValvesSetPayload : CreateValvesSetPayload
    {
        /// <summary>
        /// Creates a timestamped message that write a 1 to any bit to enable the corresponding valve.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValvesSet register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValvesSet.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that write a 1 to any bit to disable the corresponding valve.
    /// </summary>
    [DisplayName("ValvesClearPayload")]
    [Description("Creates a message payload that write a 1 to any bit to disable the corresponding valve.")]
    public partial class CreateValvesClearPayload
    {
        /// <summary>
        /// Gets or sets the value that write a 1 to any bit to disable the corresponding valve.
        /// </summary>
        [Description("The value that write a 1 to any bit to disable the corresponding valve.")]
        public ValveMask ValvesClear { get; set; }

        /// <summary>
        /// Creates a message payload for the ValvesClear register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ValveMask GetPayload()
        {
            return ValvesClear;
        }

        /// <summary>
        /// Creates a message that write a 1 to any bit to disable the corresponding valve.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValvesClear register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValvesClear.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that write a 1 to any bit to disable the corresponding valve.
    /// </summary>
    [DisplayName("TimestampedValvesClearPayload")]
    [Description("Creates a timestamped message payload that write a 1 to any bit to disable the corresponding valve.")]
    public partial class CreateTimestampedValvesClearPayload : CreateValvesClearPayload
    {
        /// <summary>
        /// Creates a timestamped message that write a 1 to any bit to disable the corresponding valve.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValvesClear register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValvesClear.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.
    /// </summary>
    [DisplayName("ValveConfig0Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.")]
    public partial class CreateValveConfig0Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.")]
        public byte[] ValveConfig0 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig0 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig0;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig0 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig0.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.
    /// </summary>
    [DisplayName("TimestampedValveConfig0Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.")]
    public partial class CreateTimestampedValveConfig0Payload : CreateValveConfig0Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve0.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig0 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig0.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.
    /// </summary>
    [DisplayName("ValveConfig1Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.")]
    public partial class CreateValveConfig1Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.")]
        public byte[] ValveConfig1 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig1 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig1;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig1 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig1.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.
    /// </summary>
    [DisplayName("TimestampedValveConfig1Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.")]
    public partial class CreateTimestampedValveConfig1Payload : CreateValveConfig1Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve1.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig1 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig1.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.
    /// </summary>
    [DisplayName("ValveConfig2Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.")]
    public partial class CreateValveConfig2Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.")]
        public byte[] ValveConfig2 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig2 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig2;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig2 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig2.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.
    /// </summary>
    [DisplayName("TimestampedValveConfig2Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.")]
    public partial class CreateTimestampedValveConfig2Payload : CreateValveConfig2Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve2.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig2 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig2.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.
    /// </summary>
    [DisplayName("ValveConfig3Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.")]
    public partial class CreateValveConfig3Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.")]
        public byte[] ValveConfig3 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig3 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig3;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig3 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig3.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.
    /// </summary>
    [DisplayName("TimestampedValveConfig3Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.")]
    public partial class CreateTimestampedValveConfig3Payload : CreateValveConfig3Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve3.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig3 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig3.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.
    /// </summary>
    [DisplayName("ValveConfig4Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.")]
    public partial class CreateValveConfig4Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.")]
        public byte[] ValveConfig4 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig4 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig4;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig4 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig4.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.
    /// </summary>
    [DisplayName("TimestampedValveConfig4Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.")]
    public partial class CreateTimestampedValveConfig4Payload : CreateValveConfig4Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve4.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig4 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig4.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.
    /// </summary>
    [DisplayName("ValveConfig5Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.")]
    public partial class CreateValveConfig5Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.")]
        public byte[] ValveConfig5 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig5 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig5;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig5 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig5.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.
    /// </summary>
    [DisplayName("TimestampedValveConfig5Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.")]
    public partial class CreateTimestampedValveConfig5Payload : CreateValveConfig5Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve5.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig5 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig5.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.
    /// </summary>
    [DisplayName("ValveConfig6Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.")]
    public partial class CreateValveConfig6Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.")]
        public byte[] ValveConfig6 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig6 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig6;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig6 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig6.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.
    /// </summary>
    [DisplayName("TimestampedValveConfig6Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.")]
    public partial class CreateTimestampedValveConfig6Payload : CreateValveConfig6Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve6.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig6 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig6.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.
    /// </summary>
    [DisplayName("ValveConfig7Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.")]
    public partial class CreateValveConfig7Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.")]
        public byte[] ValveConfig7 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig7 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig7;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig7 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig7.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.
    /// </summary>
    [DisplayName("TimestampedValveConfig7Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.")]
    public partial class CreateTimestampedValveConfig7Payload : CreateValveConfig7Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve7.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig7 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig7.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.
    /// </summary>
    [DisplayName("ValveConfig8Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.")]
    public partial class CreateValveConfig8Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.")]
        public byte[] ValveConfig8 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig8 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig8;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig8 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig8.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.
    /// </summary>
    [DisplayName("TimestampedValveConfig8Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.")]
    public partial class CreateTimestampedValveConfig8Payload : CreateValveConfig8Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve8.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig8 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig8.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.
    /// </summary>
    [DisplayName("ValveConfig9Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.")]
    public partial class CreateValveConfig9Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.")]
        public byte[] ValveConfig9 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig9 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig9;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig9 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig9.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.
    /// </summary>
    [DisplayName("TimestampedValveConfig9Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.")]
    public partial class CreateTimestampedValveConfig9Payload : CreateValveConfig9Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve9.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig9 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig9.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.
    /// </summary>
    [DisplayName("ValveConfig10Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.")]
    public partial class CreateValveConfig10Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.")]
        public byte[] ValveConfig10 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig10 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig10;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig10 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig10.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.
    /// </summary>
    [DisplayName("TimestampedValveConfig10Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.")]
    public partial class CreateTimestampedValveConfig10Payload : CreateValveConfig10Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve10.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig10 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig10.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.
    /// </summary>
    [DisplayName("ValveConfig11Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.")]
    public partial class CreateValveConfig11Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.")]
        public byte[] ValveConfig11 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig11 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig11;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig11 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig11.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.
    /// </summary>
    [DisplayName("TimestampedValveConfig11Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.")]
    public partial class CreateTimestampedValveConfig11Payload : CreateValveConfig11Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve11.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig11 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig11.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.
    /// </summary>
    [DisplayName("ValveConfig12Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.")]
    public partial class CreateValveConfig12Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.")]
        public byte[] ValveConfig12 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig12 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig12;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig12 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig12.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.
    /// </summary>
    [DisplayName("TimestampedValveConfig12Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.")]
    public partial class CreateTimestampedValveConfig12Payload : CreateValveConfig12Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve12.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig12 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig12.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.
    /// </summary>
    [DisplayName("ValveConfig13Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.")]
    public partial class CreateValveConfig13Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.")]
        public byte[] ValveConfig13 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig13 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig13;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig13 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig13.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.
    /// </summary>
    [DisplayName("TimestampedValveConfig13Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.")]
    public partial class CreateTimestampedValveConfig13Payload : CreateValveConfig13Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve13.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig13 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig13.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.
    /// </summary>
    [DisplayName("ValveConfig14Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.")]
    public partial class CreateValveConfig14Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.")]
        public byte[] ValveConfig14 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig14 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig14;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig14 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig14.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.
    /// </summary>
    [DisplayName("TimestampedValveConfig14Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.")]
    public partial class CreateTimestampedValveConfig14Payload : CreateValveConfig14Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve14.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig14 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig14.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.
    /// </summary>
    [DisplayName("ValveConfig15Payload")]
    [Description("Creates a message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.")]
    public partial class CreateValveConfig15Payload
    {
        /// <summary>
        /// Gets or sets the value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.
        /// </summary>
        [Description("The value that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.")]
        public byte[] ValveConfig15 { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveConfig15 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return ValveConfig15;
        }

        /// <summary>
        /// Creates a message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ValveConfig15 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig15.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.
    /// </summary>
    [DisplayName("TimestampedValveConfig15Payload")]
    [Description("Creates a timestamped message payload that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.")]
    public partial class CreateTimestampedValveConfig15Payload : CreateValveConfig15Payload
    {
        /// <summary>
        /// Creates a timestamped message that the hit duty cycle (float: 0 - 1.0), hold duty cycle (float: 0 - 1.0), and hit duration in microseconds (U32) for Valve15.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ValveConfig15 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ValveConfig15.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specify each auxiliary GPIO pin as an input (0) or output (1).
    /// </summary>
    [DisplayName("AuxGPIODirPayload")]
    [Description("Creates a message payload that specify each auxiliary GPIO pin as an input (0) or output (1).")]
    public partial class CreateAuxGPIODirPayload
    {
        /// <summary>
        /// Gets or sets the value that specify each auxiliary GPIO pin as an input (0) or output (1).
        /// </summary>
        [Description("The value that specify each auxiliary GPIO pin as an input (0) or output (1).")]
        public AuxGPIOMask AuxGPIODir { get; set; }

        /// <summary>
        /// Creates a message payload for the AuxGPIODir register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public AuxGPIOMask GetPayload()
        {
            return AuxGPIODir;
        }

        /// <summary>
        /// Creates a message that specify each auxiliary GPIO pin as an input (0) or output (1).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the AuxGPIODir register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIODir.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specify each auxiliary GPIO pin as an input (0) or output (1).
    /// </summary>
    [DisplayName("TimestampedAuxGPIODirPayload")]
    [Description("Creates a timestamped message payload that specify each auxiliary GPIO pin as an input (0) or output (1).")]
    public partial class CreateTimestampedAuxGPIODirPayload : CreateAuxGPIODirPayload
    {
        /// <summary>
        /// Creates a timestamped message that specify each auxiliary GPIO pin as an input (0) or output (1).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the AuxGPIODir register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIODir.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the state (one or off) of any auxiliary GPIO pins specified as outputs.
    /// </summary>
    [DisplayName("AuxGPIOStatePayload")]
    [Description("Creates a message payload that set the state (one or off) of any auxiliary GPIO pins specified as outputs.")]
    public partial class CreateAuxGPIOStatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the state (one or off) of any auxiliary GPIO pins specified as outputs.
        /// </summary>
        [Description("The value that set the state (one or off) of any auxiliary GPIO pins specified as outputs.")]
        public AuxGPIOMask AuxGPIOState { get; set; }

        /// <summary>
        /// Creates a message payload for the AuxGPIOState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public AuxGPIOMask GetPayload()
        {
            return AuxGPIOState;
        }

        /// <summary>
        /// Creates a message that set the state (one or off) of any auxiliary GPIO pins specified as outputs.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the AuxGPIOState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the state (one or off) of any auxiliary GPIO pins specified as outputs.
    /// </summary>
    [DisplayName("TimestampedAuxGPIOStatePayload")]
    [Description("Creates a timestamped message payload that set the state (one or off) of any auxiliary GPIO pins specified as outputs.")]
    public partial class CreateTimestampedAuxGPIOStatePayload : CreateAuxGPIOStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the state (one or off) of any auxiliary GPIO pins specified as outputs.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the AuxGPIOState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that when writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.
    /// </summary>
    [DisplayName("AuxGPIOSetPayload")]
    [Description("Creates a message payload that when writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.")]
    public partial class CreateAuxGPIOSetPayload
    {
        /// <summary>
        /// Gets or sets the value that when writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.
        /// </summary>
        [Description("The value that when writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.")]
        public AuxGPIOMask AuxGPIOSet { get; set; }

        /// <summary>
        /// Creates a message payload for the AuxGPIOSet register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public AuxGPIOMask GetPayload()
        {
            return AuxGPIOSet;
        }

        /// <summary>
        /// Creates a message that when writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the AuxGPIOSet register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOSet.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that when writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.
    /// </summary>
    [DisplayName("TimestampedAuxGPIOSetPayload")]
    [Description("Creates a timestamped message payload that when writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.")]
    public partial class CreateTimestampedAuxGPIOSetPayload : CreateAuxGPIOSetPayload
    {
        /// <summary>
        /// Creates a timestamped message that when writing a 1 to any bit, turn on the specified auxiliary GPIO pins specified as outputs.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the AuxGPIOSet register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOSet.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that when writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.
    /// </summary>
    [DisplayName("AuxGPIOClearPayload")]
    [Description("Creates a message payload that when writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.")]
    public partial class CreateAuxGPIOClearPayload
    {
        /// <summary>
        /// Gets or sets the value that when writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.
        /// </summary>
        [Description("The value that when writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.")]
        public AuxGPIOMask AuxGPIOClear { get; set; }

        /// <summary>
        /// Creates a message payload for the AuxGPIOClear register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public AuxGPIOMask GetPayload()
        {
            return AuxGPIOClear;
        }

        /// <summary>
        /// Creates a message that when writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the AuxGPIOClear register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOClear.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that when writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.
    /// </summary>
    [DisplayName("TimestampedAuxGPIOClearPayload")]
    [Description("Creates a timestamped message payload that when writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.")]
    public partial class CreateTimestampedAuxGPIOClearPayload : CreateAuxGPIOClearPayload
    {
        /// <summary>
        /// Creates a timestamped message that when writing a 1 to any bit, Turn off the specified auxiliary GPIO pins specified as outputs.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the AuxGPIOClear register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOClear.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// for register AuxGPIOInputRiseEvent.
    /// </summary>
    [DisplayName("AuxGPIOInputRiseEventPayload")]
    [Description("Creates a message payload for register AuxGPIOInputRiseEvent.")]
    public partial class CreateAuxGPIOInputRiseEventPayload
    {
        /// <summary>
        /// Gets or sets the value for register AuxGPIOInputRiseEvent.
        /// </summary>
        [Description("The value for register AuxGPIOInputRiseEvent.")]
        public AuxGPIOMask AuxGPIOInputRiseEvent { get; set; }

        /// <summary>
        /// Creates a message payload for the AuxGPIOInputRiseEvent register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public AuxGPIOMask GetPayload()
        {
            return AuxGPIOInputRiseEvent;
        }

        /// <summary>
        /// Creates a message for register AuxGPIOInputRiseEvent.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the AuxGPIOInputRiseEvent register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOInputRiseEvent.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// for register AuxGPIOInputRiseEvent.
    /// </summary>
    [DisplayName("TimestampedAuxGPIOInputRiseEventPayload")]
    [Description("Creates a timestamped message payload for register AuxGPIOInputRiseEvent.")]
    public partial class CreateTimestampedAuxGPIOInputRiseEventPayload : CreateAuxGPIOInputRiseEventPayload
    {
        /// <summary>
        /// Creates a timestamped message for register AuxGPIOInputRiseEvent.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the AuxGPIOInputRiseEvent register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOInputRiseEvent.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// for register AuxGPIOInputFallEvent.
    /// </summary>
    [DisplayName("AuxGPIOInputFallEventPayload")]
    [Description("Creates a message payload for register AuxGPIOInputFallEvent.")]
    public partial class CreateAuxGPIOInputFallEventPayload
    {
        /// <summary>
        /// Gets or sets the value for register AuxGPIOInputFallEvent.
        /// </summary>
        [Description("The value for register AuxGPIOInputFallEvent.")]
        public AuxGPIOMask AuxGPIOInputFallEvent { get; set; }

        /// <summary>
        /// Creates a message payload for the AuxGPIOInputFallEvent register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public AuxGPIOMask GetPayload()
        {
            return AuxGPIOInputFallEvent;
        }

        /// <summary>
        /// Creates a message for register AuxGPIOInputFallEvent.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the AuxGPIOInputFallEvent register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOInputFallEvent.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// for register AuxGPIOInputFallEvent.
    /// </summary>
    [DisplayName("TimestampedAuxGPIOInputFallEventPayload")]
    [Description("Creates a timestamped message payload for register AuxGPIOInputFallEvent.")]
    public partial class CreateTimestampedAuxGPIOInputFallEventPayload : CreateAuxGPIOInputFallEventPayload
    {
        /// <summary>
        /// Creates a timestamped message for register AuxGPIOInputFallEvent.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the AuxGPIOInputFallEvent register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOInputFallEvent.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// for register AuxGPIORisingInputs.
    /// </summary>
    [DisplayName("AuxGPIORisingInputsPayload")]
    [Description("Creates a message payload for register AuxGPIORisingInputs.")]
    public partial class CreateAuxGPIORisingInputsPayload
    {
        /// <summary>
        /// Gets or sets the value for register AuxGPIORisingInputs.
        /// </summary>
        [Description("The value for register AuxGPIORisingInputs.")]
        public AuxGPIOMask AuxGPIORisingInputs { get; set; }

        /// <summary>
        /// Creates a message payload for the AuxGPIORisingInputs register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public AuxGPIOMask GetPayload()
        {
            return AuxGPIORisingInputs;
        }

        /// <summary>
        /// Creates a message for register AuxGPIORisingInputs.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the AuxGPIORisingInputs register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIORisingInputs.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// for register AuxGPIORisingInputs.
    /// </summary>
    [DisplayName("TimestampedAuxGPIORisingInputsPayload")]
    [Description("Creates a timestamped message payload for register AuxGPIORisingInputs.")]
    public partial class CreateTimestampedAuxGPIORisingInputsPayload : CreateAuxGPIORisingInputsPayload
    {
        /// <summary>
        /// Creates a timestamped message for register AuxGPIORisingInputs.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the AuxGPIORisingInputs register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIORisingInputs.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// for register AuxGPIOFallingInputs.
    /// </summary>
    [DisplayName("AuxGPIOFallingInputsPayload")]
    [Description("Creates a message payload for register AuxGPIOFallingInputs.")]
    public partial class CreateAuxGPIOFallingInputsPayload
    {
        /// <summary>
        /// Gets or sets the value for register AuxGPIOFallingInputs.
        /// </summary>
        [Description("The value for register AuxGPIOFallingInputs.")]
        public AuxGPIOMask AuxGPIOFallingInputs { get; set; }

        /// <summary>
        /// Creates a message payload for the AuxGPIOFallingInputs register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public AuxGPIOMask GetPayload()
        {
            return AuxGPIOFallingInputs;
        }

        /// <summary>
        /// Creates a message for register AuxGPIOFallingInputs.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the AuxGPIOFallingInputs register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOFallingInputs.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// for register AuxGPIOFallingInputs.
    /// </summary>
    [DisplayName("TimestampedAuxGPIOFallingInputsPayload")]
    [Description("Creates a timestamped message payload for register AuxGPIOFallingInputs.")]
    public partial class CreateTimestampedAuxGPIOFallingInputsPayload : CreateAuxGPIOFallingInputsPayload
    {
        /// <summary>
        /// Creates a timestamped message for register AuxGPIOFallingInputs.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the AuxGPIOFallingInputs register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.AuxGPIOFallingInputs.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that which poke ports are active.
    /// </summary>
    [DisplayName("PokePinPayload")]
    [Description("Creates a message payload that which poke ports are active.")]
    public partial class CreatePokePinPayload
    {
        /// <summary>
        /// Gets or sets the value that which poke ports are active.
        /// </summary>
        [Description("The value that which poke ports are active.")]
        public byte PokePin { get; set; }

        /// <summary>
        /// Creates a message payload for the PokePin register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return PokePin;
        }

        /// <summary>
        /// Creates a message that which poke ports are active.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PokePin register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PokePin.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that which poke ports are active.
    /// </summary>
    [DisplayName("TimestampedPokePinPayload")]
    [Description("Creates a timestamped message payload that which poke ports are active.")]
    public partial class CreateTimestampedPokePinPayload : CreatePokePinPayload
    {
        /// <summary>
        /// Creates a timestamped message that which poke ports are active.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PokePin register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PokePin.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).
    /// </summary>
    [DisplayName("PokePinInvertedPayload")]
    [Description("Creates a message payload that which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).")]
    public partial class CreatePokePinInvertedPayload
    {
        /// <summary>
        /// Gets or sets the value that which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).
        /// </summary>
        [Description("The value that which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).")]
        public byte PokePinInverted { get; set; }

        /// <summary>
        /// Creates a message payload for the PokePinInverted register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return PokePinInverted;
        }

        /// <summary>
        /// Creates a message that which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PokePinInverted register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PokePinInverted.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).
    /// </summary>
    [DisplayName("TimestampedPokePinInvertedPayload")]
    [Description("Creates a timestamped message payload that which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).")]
    public partial class CreateTimestampedPokePinInvertedPayload : CreatePokePinInvertedPayload
    {
        /// <summary>
        /// Creates a timestamped message that which poke ports are inverted (i.e: transition from HIGH to LOW when a poke occurs).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PokePinInverted register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PokePinInverted.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.
    /// </summary>
    [DisplayName("PokeStatePayload")]
    [Description("Creates a message payload that the state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.")]
    public partial class CreatePokeStatePayload
    {
        /// <summary>
        /// Gets or sets the value that the state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.
        /// </summary>
        [Description("The value that the state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.")]
        public byte PokeState { get; set; }

        /// <summary>
        /// Creates a message payload for the PokeState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return PokeState;
        }

        /// <summary>
        /// Creates a message that the state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PokeState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PokeState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.
    /// </summary>
    [DisplayName("TimestampedPokeStatePayload")]
    [Description("Creates a timestamped message payload that the state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.")]
    public partial class CreateTimestampedPokeStatePayload : CreatePokeStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that the state of the poke port. An event will be triggered given a poke/ beam break that is greater than the min poke time.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PokeState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PokeState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).
    /// </summary>
    [DisplayName("RawPokeStatePayload")]
    [Description("Creates a message payload that the raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).")]
    public partial class CreateRawPokeStatePayload
    {
        /// <summary>
        /// Gets or sets the value that the raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).
        /// </summary>
        [Description("The value that the raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).")]
        public byte RawPokeState { get; set; }

        /// <summary>
        /// Creates a message payload for the RawPokeState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return RawPokeState;
        }

        /// <summary>
        /// Creates a message that the raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the RawPokeState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.RawPokeState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).
    /// </summary>
    [DisplayName("TimestampedRawPokeStatePayload")]
    [Description("Creates a timestamped message payload that the raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).")]
    public partial class CreateTimestampedRawPokeStatePayload : CreateRawPokeStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that the raw state of the poke pin. Events will be triggered at the onset of a beam break (1) and offset (0).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the RawPokeState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.RawPokeState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that number of mouse pokes per port since boot or reset.
    /// </summary>
    [DisplayName("PokeDometerPayload")]
    [Description("Creates a message payload that number of mouse pokes per port since boot or reset.")]
    public partial class CreatePokeDometerPayload
    {
        /// <summary>
        /// Gets or sets the value that number of mouse pokes per port since boot or reset.
        /// </summary>
        [Description("The value that number of mouse pokes per port since boot or reset.")]
        public uint PokeDometer { get; set; }

        /// <summary>
        /// Creates a message payload for the PokeDometer register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return PokeDometer;
        }

        /// <summary>
        /// Creates a message that number of mouse pokes per port since boot or reset.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PokeDometer register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PokeDometer.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that number of mouse pokes per port since boot or reset.
    /// </summary>
    [DisplayName("TimestampedPokeDometerPayload")]
    [Description("Creates a timestamped message payload that number of mouse pokes per port since boot or reset.")]
    public partial class CreateTimestampedPokeDometerPayload : CreatePokeDometerPayload
    {
        /// <summary>
        /// Creates a timestamped message that number of mouse pokes per port since boot or reset.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PokeDometer register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PokeDometer.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
    /// </summary>
    [DisplayName("FSMStatePayload")]
    [Description("Creates a message payload that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.")]
    public partial class CreateFSMStatePayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
        /// </summary>
        [Description("The value that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.")]
        public byte FSMState { get; set; }

        /// <summary>
        /// Creates a message payload for the FSMState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return FSMState;
        }

        /// <summary>
        /// Creates a message that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the FSMState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FSMState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
    /// </summary>
    [DisplayName("TimestampedFSMStatePayload")]
    [Description("Creates a timestamped message payload that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.")]
    public partial class CreateTimestampedFSMStatePayload : CreateFSMStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that CurrentOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the FSMState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FSMState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.
    /// </summary>
    [DisplayName("ForceFSMPayload")]
    [Description("Creates a message payload that force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.")]
    public partial class CreateForceFSMPayload
    {
        /// <summary>
        /// Gets or sets the value that force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.
        /// </summary>
        [Description("The value that force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.")]
        public byte ForceFSM { get; set; }

        /// <summary>
        /// Creates a message payload for the ForceFSM register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return ForceFSM;
        }

        /// <summary>
        /// Creates a message that force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ForceFSM register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ForceFSM.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.
    /// </summary>
    [DisplayName("TimestampedForceFSMPayload")]
    [Description("Creates a timestamped message payload that force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.")]
    public partial class CreateTimestampedForceFSMPayload : CreateForceFSMPayload
    {
        /// <summary>
        /// Creates a timestamped message that force the poke handling state machine to iterate as if handling a mouse poke. PokeDometers are not incremented.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ForceFSM register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ForceFSM.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed.
    /// </summary>
    [DisplayName("QueuedOdorIndexPayload")]
    [Description("Creates a message payload that queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed.")]
    public partial class CreateQueuedOdorIndexPayload
    {
        /// <summary>
        /// Gets or sets the value that queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed.
        /// </summary>
        [Description("The value that queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed.")]
        public sbyte QueuedOdorIndex { get; set; }

        /// <summary>
        /// Creates a message payload for the QueuedOdorIndex register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public sbyte GetPayload()
        {
            return QueuedOdorIndex;
        }

        /// <summary>
        /// Creates a message that queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the QueuedOdorIndex register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.QueuedOdorIndex.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed.
    /// </summary>
    [DisplayName("TimestampedQueuedOdorIndexPayload")]
    [Description("Creates a timestamped message payload that queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed.")]
    public partial class CreateTimestampedQueuedOdorIndexPayload : CreateQueuedOdorIndexPayload
    {
        /// <summary>
        /// Creates a timestamped message that queued odor (value: odor valve index) that will be delievered to the odor port given a register poke. After an odor has been dispensed, the register will be set to -1, which indicates that a new odor is needed.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the QueuedOdorIndex register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.QueuedOdorIndex.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time alotted (in microseconds) for the vacuum valve to close.
    /// </summary>
    [DisplayName("VacuumCloseTimeUSPayload")]
    [Description("Creates a message payload that time alotted (in microseconds) for the vacuum valve to close.")]
    public partial class CreateVacuumCloseTimeUSPayload
    {
        /// <summary>
        /// Gets or sets the value that time alotted (in microseconds) for the vacuum valve to close.
        /// </summary>
        [Description("The value that time alotted (in microseconds) for the vacuum valve to close.")]
        public uint VacuumCloseTimeUS { get; set; }

        /// <summary>
        /// Creates a message payload for the VacuumCloseTimeUS register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return VacuumCloseTimeUS;
        }

        /// <summary>
        /// Creates a message that time alotted (in microseconds) for the vacuum valve to close.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the VacuumCloseTimeUS register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.VacuumCloseTimeUS.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time alotted (in microseconds) for the vacuum valve to close.
    /// </summary>
    [DisplayName("TimestampedVacuumCloseTimeUSPayload")]
    [Description("Creates a timestamped message payload that time alotted (in microseconds) for the vacuum valve to close.")]
    public partial class CreateTimestampedVacuumCloseTimeUSPayload : CreateVacuumCloseTimeUSPayload
    {
        /// <summary>
        /// Creates a timestamped message that time alotted (in microseconds) for the vacuum valve to close.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the VacuumCloseTimeUS register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.VacuumCloseTimeUS.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that minimum time alotted (in microseconds) for the odor delivery state.
    /// </summary>
    [DisplayName("MinOdorDeliveryTimeUSPayload")]
    [Description("Creates a message payload that minimum time alotted (in microseconds) for the odor delivery state.")]
    public partial class CreateMinOdorDeliveryTimeUSPayload
    {
        /// <summary>
        /// Gets or sets the value that minimum time alotted (in microseconds) for the odor delivery state.
        /// </summary>
        [Description("The value that minimum time alotted (in microseconds) for the odor delivery state.")]
        public uint MinOdorDeliveryTimeUS { get; set; }

        /// <summary>
        /// Creates a message payload for the MinOdorDeliveryTimeUS register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return MinOdorDeliveryTimeUS;
        }

        /// <summary>
        /// Creates a message that minimum time alotted (in microseconds) for the odor delivery state.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the MinOdorDeliveryTimeUS register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.MinOdorDeliveryTimeUS.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that minimum time alotted (in microseconds) for the odor delivery state.
    /// </summary>
    [DisplayName("TimestampedMinOdorDeliveryTimeUSPayload")]
    [Description("Creates a timestamped message payload that minimum time alotted (in microseconds) for the odor delivery state.")]
    public partial class CreateTimestampedMinOdorDeliveryTimeUSPayload : CreateMinOdorDeliveryTimeUSPayload
    {
        /// <summary>
        /// Creates a timestamped message that minimum time alotted (in microseconds) for the odor delivery state.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the MinOdorDeliveryTimeUS register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.MinOdorDeliveryTimeUS.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that maximum time alotted (in microseconds) for the odor delivery state.
    /// </summary>
    [DisplayName("MaxOdorDeliveryTimeUSPayload")]
    [Description("Creates a message payload that maximum time alotted (in microseconds) for the odor delivery state.")]
    public partial class CreateMaxOdorDeliveryTimeUSPayload
    {
        /// <summary>
        /// Gets or sets the value that maximum time alotted (in microseconds) for the odor delivery state.
        /// </summary>
        [Description("The value that maximum time alotted (in microseconds) for the odor delivery state.")]
        public uint MaxOdorDeliveryTimeUS { get; set; }

        /// <summary>
        /// Creates a message payload for the MaxOdorDeliveryTimeUS register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return MaxOdorDeliveryTimeUS;
        }

        /// <summary>
        /// Creates a message that maximum time alotted (in microseconds) for the odor delivery state.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the MaxOdorDeliveryTimeUS register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.MaxOdorDeliveryTimeUS.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that maximum time alotted (in microseconds) for the odor delivery state.
    /// </summary>
    [DisplayName("TimestampedMaxOdorDeliveryTimeUSPayload")]
    [Description("Creates a timestamped message payload that maximum time alotted (in microseconds) for the odor delivery state.")]
    public partial class CreateTimestampedMaxOdorDeliveryTimeUSPayload : CreateMaxOdorDeliveryTimeUSPayload
    {
        /// <summary>
        /// Creates a timestamped message that maximum time alotted (in microseconds) for the odor delivery state.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the MaxOdorDeliveryTimeUS register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.MaxOdorDeliveryTimeUS.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time alotted (in microseconds) before the vacuum turns on to remove the current odor.
    /// </summary>
    [DisplayName("OdorTransitionTimeUSPayload")]
    [Description("Creates a message payload that time alotted (in microseconds) before the vacuum turns on to remove the current odor.")]
    public partial class CreateOdorTransitionTimeUSPayload
    {
        /// <summary>
        /// Gets or sets the value that time alotted (in microseconds) before the vacuum turns on to remove the current odor.
        /// </summary>
        [Description("The value that time alotted (in microseconds) before the vacuum turns on to remove the current odor.")]
        public uint OdorTransitionTimeUS { get; set; }

        /// <summary>
        /// Creates a message payload for the OdorTransitionTimeUS register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return OdorTransitionTimeUS;
        }

        /// <summary>
        /// Creates a message that time alotted (in microseconds) before the vacuum turns on to remove the current odor.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OdorTransitionTimeUS register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.OdorTransitionTimeUS.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time alotted (in microseconds) before the vacuum turns on to remove the current odor.
    /// </summary>
    [DisplayName("TimestampedOdorTransitionTimeUSPayload")]
    [Description("Creates a timestamped message payload that time alotted (in microseconds) before the vacuum turns on to remove the current odor.")]
    public partial class CreateTimestampedOdorTransitionTimeUSPayload : CreateOdorTransitionTimeUSPayload
    {
        /// <summary>
        /// Creates a timestamped message that time alotted (in microseconds) before the vacuum turns on to remove the current odor.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OdorTransitionTimeUS register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.OdorTransitionTimeUS.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time alotted (in microseconds) for the vacuum to open.
    /// </summary>
    [DisplayName("VacuumSetupTimeUSPayload")]
    [Description("Creates a message payload that time alotted (in microseconds) for the vacuum to open.")]
    public partial class CreateVacuumSetupTimeUSPayload
    {
        /// <summary>
        /// Gets or sets the value that time alotted (in microseconds) for the vacuum to open.
        /// </summary>
        [Description("The value that time alotted (in microseconds) for the vacuum to open.")]
        public uint VacuumSetupTimeUS { get; set; }

        /// <summary>
        /// Creates a message payload for the VacuumSetupTimeUS register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return VacuumSetupTimeUS;
        }

        /// <summary>
        /// Creates a message that time alotted (in microseconds) for the vacuum to open.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the VacuumSetupTimeUS register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.VacuumSetupTimeUS.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time alotted (in microseconds) for the vacuum to open.
    /// </summary>
    [DisplayName("TimestampedVacuumSetupTimeUSPayload")]
    [Description("Creates a timestamped message payload that time alotted (in microseconds) for the vacuum to open.")]
    public partial class CreateTimestampedVacuumSetupTimeUSPayload : CreateVacuumSetupTimeUSPayload
    {
        /// <summary>
        /// Creates a timestamped message that time alotted (in microseconds) for the vacuum to open.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the VacuumSetupTimeUS register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.VacuumSetupTimeUS.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time alotted (in microseconds) for the final valve to open and remain on.
    /// </summary>
    [DisplayName("FinalValveEnergizedTimeUSPayload")]
    [Description("Creates a message payload that time alotted (in microseconds) for the final valve to open and remain on.")]
    public partial class CreateFinalValveEnergizedTimeUSPayload
    {
        /// <summary>
        /// Gets or sets the value that time alotted (in microseconds) for the final valve to open and remain on.
        /// </summary>
        [Description("The value that time alotted (in microseconds) for the final valve to open and remain on.")]
        public uint FinalValveEnergizedTimeUS { get; set; }

        /// <summary>
        /// Creates a message payload for the FinalValveEnergizedTimeUS register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return FinalValveEnergizedTimeUS;
        }

        /// <summary>
        /// Creates a message that time alotted (in microseconds) for the final valve to open and remain on.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the FinalValveEnergizedTimeUS register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FinalValveEnergizedTimeUS.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time alotted (in microseconds) for the final valve to open and remain on.
    /// </summary>
    [DisplayName("TimestampedFinalValveEnergizedTimeUSPayload")]
    [Description("Creates a timestamped message payload that time alotted (in microseconds) for the final valve to open and remain on.")]
    public partial class CreateTimestampedFinalValveEnergizedTimeUSPayload : CreateFinalValveEnergizedTimeUSPayload
    {
        /// <summary>
        /// Creates a timestamped message that time alotted (in microseconds) for the final valve to open and remain on.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the FinalValveEnergizedTimeUS register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FinalValveEnergizedTimeUS.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.
    /// </summary>
    [DisplayName("MinimumPokeTimeUSPayload")]
    [Description("Creates a message payload that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.")]
    public partial class CreateMinimumPokeTimeUSPayload
    {
        /// <summary>
        /// Gets or sets the value that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.
        /// </summary>
        [Description("The value that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.")]
        public uint MinimumPokeTimeUS { get; set; }

        /// <summary>
        /// Creates a message payload for the MinimumPokeTimeUS register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return MinimumPokeTimeUS;
        }

        /// <summary>
        /// Creates a message that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the MinimumPokeTimeUS register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.MinimumPokeTimeUS.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.
    /// </summary>
    [DisplayName("TimestampedMinimumPokeTimeUSPayload")]
    [Description("Creates a timestamped message payload that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.")]
    public partial class CreateTimestampedMinimumPokeTimeUSPayload : CreateMinimumPokeTimeUSPayload
    {
        /// <summary>
        /// Creates a timestamped message that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the MinimumPokeTimeUS register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.MinimumPokeTimeUS.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the GPIO output pin used for camera triggering. Default pin is 26.
    /// </summary>
    [DisplayName("CamPinPayload")]
    [Description("Creates a message payload that the GPIO output pin used for camera triggering. Default pin is 26.")]
    public partial class CreateCamPinPayload
    {
        /// <summary>
        /// Gets or sets the value that the GPIO output pin used for camera triggering. Default pin is 26.
        /// </summary>
        [Description("The value that the GPIO output pin used for camera triggering. Default pin is 26.")]
        public byte CamPin { get; set; }

        /// <summary>
        /// Creates a message payload for the CamPin register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return CamPin;
        }

        /// <summary>
        /// Creates a message that the GPIO output pin used for camera triggering. Default pin is 26.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the CamPin register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.CamPin.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the GPIO output pin used for camera triggering. Default pin is 26.
    /// </summary>
    [DisplayName("TimestampedCamPinPayload")]
    [Description("Creates a timestamped message payload that the GPIO output pin used for camera triggering. Default pin is 26.")]
    public partial class CreateTimestampedCamPinPayload : CreateCamPinPayload
    {
        /// <summary>
        /// Creates a timestamped message that the GPIO output pin used for camera triggering. Default pin is 26.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the CamPin register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.CamPin.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
    /// </summary>
    [DisplayName("CamPinStatePayload")]
    [Description("Creates a message payload that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
    public partial class CreateCamPinStatePayload
    {
        /// <summary>
        /// Gets or sets the value that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
        /// </summary>
        [Description("The value that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
        public byte CamPinState { get; set; }

        /// <summary>
        /// Creates a message payload for the CamPinState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return CamPinState;
        }

        /// <summary>
        /// Creates a message that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the CamPinState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.CamPinState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
    /// </summary>
    [DisplayName("TimestampedCamPinStatePayload")]
    [Description("Creates a timestamped message payload that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
    public partial class CreateTimestampedCamPinStatePayload : CreateCamPinStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the CamPinState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.CamPinState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the frame rate of the camera trigger/ frequency of the PWM signal.
    /// </summary>
    [DisplayName("FrameRatePayload")]
    [Description("Creates a message payload that set the frame rate of the camera trigger/ frequency of the PWM signal.")]
    public partial class CreateFrameRatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the frame rate of the camera trigger/ frequency of the PWM signal.
        /// </summary>
        [Description("The value that set the frame rate of the camera trigger/ frequency of the PWM signal.")]
        public uint FrameRate { get; set; }

        /// <summary>
        /// Creates a message payload for the FrameRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return FrameRate;
        }

        /// <summary>
        /// Creates a message that set the frame rate of the camera trigger/ frequency of the PWM signal.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the FrameRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FrameRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the frame rate of the camera trigger/ frequency of the PWM signal.
    /// </summary>
    [DisplayName("TimestampedFrameRatePayload")]
    [Description("Creates a timestamped message payload that set the frame rate of the camera trigger/ frequency of the PWM signal.")]
    public partial class CreateTimestampedFrameRatePayload : CreateFrameRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the frame rate of the camera trigger/ frequency of the PWM signal.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the FrameRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FrameRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
    /// </summary>
    [DisplayName("DutyCyclePayload")]
    [Description("Creates a message payload that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
    public partial class CreateDutyCyclePayload
    {
        /// <summary>
        /// Gets or sets the value that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
        /// </summary>
        [Description("The value that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
        public float DutyCycle { get; set; }

        /// <summary>
        /// Creates a message payload for the DutyCycle register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return DutyCycle;
        }

        /// <summary>
        /// Creates a message that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the DutyCycle register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.DutyCycle.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
    /// </summary>
    [DisplayName("TimestampedDutyCyclePayload")]
    [Description("Creates a timestamped message payload that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
    public partial class CreateTimestampedDutyCyclePayload : CreateDutyCyclePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the DutyCycle register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.DutyCycle.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable (1) and disable (0) camera triggering/ the PWM signal.
    /// </summary>
    [DisplayName("EnableCamTriggerPayload")]
    [Description("Creates a message payload that enable (1) and disable (0) camera triggering/ the PWM signal.")]
    public partial class CreateEnableCamTriggerPayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) and disable (0) camera triggering/ the PWM signal.
        /// </summary>
        [Description("The value that enable (1) and disable (0) camera triggering/ the PWM signal.")]
        public byte EnableCamTrigger { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableCamTrigger register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return EnableCamTrigger;
        }

        /// <summary>
        /// Creates a message that enable (1) and disable (0) camera triggering/ the PWM signal.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableCamTrigger register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableCamTrigger.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable (1) and disable (0) camera triggering/ the PWM signal.
    /// </summary>
    [DisplayName("TimestampedEnableCamTriggerPayload")]
    [Description("Creates a timestamped message payload that enable (1) and disable (0) camera triggering/ the PWM signal.")]
    public partial class CreateTimestampedEnableCamTriggerPayload : CreateEnableCamTriggerPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) and disable (0) camera triggering/ the PWM signal.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableCamTrigger register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableCamTrigger.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable (1) and disable (0) valve LEDs.
    /// </summary>
    [DisplayName("EnableValveLedsPayload")]
    [Description("Creates a message payload that enable (1) and disable (0) valve LEDs.")]
    public partial class CreateEnableValveLedsPayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) and disable (0) valve LEDs.
        /// </summary>
        [Description("The value that enable (1) and disable (0) valve LEDs.")]
        public byte EnableValveLeds { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableValveLeds register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return EnableValveLeds;
        }

        /// <summary>
        /// Creates a message that enable (1) and disable (0) valve LEDs.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableValveLeds register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableValveLeds.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable (1) and disable (0) valve LEDs.
    /// </summary>
    [DisplayName("TimestampedEnableValveLedsPayload")]
    [Description("Creates a timestamped message payload that enable (1) and disable (0) valve LEDs.")]
    public partial class CreateTimestampedEnableValveLedsPayload : CreateEnableValveLedsPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) and disable (0) valve LEDs.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableValveLeds register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableValveLeds.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Valve that can be configured/enabled/disabled
    /// </summary>
    public enum ValveMask : byte
    {
        Valve0 = 0,
        Valve1 = 1,
        Valve2 = 2,
        Valve3 = 3,
        Valve4 = 4,
        Valve5 = 5,
        Valve6 = 6,
        Valve7 = 7,
        Valve8 = 8,
        Valve9 = 9,
        Valve10 = 10,
        Valve11 = 11,
        Valve12 = 12,
        Valve13 = 13,
        Valve14 = 14,
        Valve15 = 15
    }

    /// <summary>
    /// Auxiliary GPIO index.
    /// </summary>
    public enum AuxGPIOMask : byte
    {
        AuxGPIO0 = 0,
        AuxGPIO1 = 1,
        AuxGPIO2 = 2,
        AuxGPIO3 = 3,
        AuxGPIO4 = 4,
        AuxGPIO5 = 5,
        AuxGPIO6 = 6,
        AuxGPIO7 = 7
    }
}
