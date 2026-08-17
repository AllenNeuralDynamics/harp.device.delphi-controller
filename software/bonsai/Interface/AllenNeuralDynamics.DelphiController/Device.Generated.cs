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
        public const int WhoAmI = 1409;

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
            { 66, typeof(QueuedOdorMask) },
            { 67, typeof(OdorSetupTimeUS) },
            { 68, typeof(MinOdorDeliveryTimeUS) },
            { 69, typeof(MaxOdorDeliveryTimeUS) },
            { 70, typeof(MinimumPokeTimeUS) },
            { 71, typeof(OdorDwellTimeUS) },
            { 72, typeof(Cam0PinState) },
            { 73, typeof(Cam0FrameRate) },
            { 74, typeof(Cam0DutyCycle) },
            { 75, typeof(EnableCam0Trigger) },
            { 76, typeof(Cam1PinState) },
            { 77, typeof(Cam1FrameRate) },
            { 78, typeof(Cam1DutyCycle) },
            { 79, typeof(EnableCam1Trigger) },
            { 80, typeof(EnableValveLeds) },
            { 81, typeof(LatestFlowRate) },
            { 82, typeof(LatestRawAdcSample) },
            { 83, typeof(EnableAdcSampling) },
            { 84, typeof(LeakAdcChannel) },
            { 85, typeof(LeakThreshold) },
            { 86, typeof(LeakState) },
            { 87, typeof(ManualFlowMeter) },
            { 88, typeof(NominalFlowRate) },
            { 89, typeof(FlowRateTolerance) },
            { 90, typeof(ManualFlowMeterState) },
            { 91, typeof(FlowMeterCalibrations) },
            { 92, typeof(PidUpdateFrequency) },
            { 93, typeof(PidGains) },
            { 94, typeof(ProportionalValve0Adc) },
            { 95, typeof(ProportionalValve0EnablePid) },
            { 96, typeof(ProportionalValve0DutyCycle) },
            { 97, typeof(ProportionalValve0TargetFlowRate) },
            { 98, typeof(ProportionalValve1Adc) },
            { 99, typeof(ProportionalValve1EnablePid) },
            { 100, typeof(ProportionalValve1DutyCycle) },
            { 101, typeof(ProportionalValve1TargetFlowRate) },
            { 102, typeof(ProportionalValve2Adc) },
            { 103, typeof(ProportionalValve2EnablePid) },
            { 104, typeof(ProportionalValve2DutyCycle) },
            { 105, typeof(ProportionalValve2TargetFlowRate) },
            { 106, typeof(FreezePidUpdates) }
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
    /// <seealso cref="QueuedOdorMask"/>
    /// <seealso cref="OdorSetupTimeUS"/>
    /// <seealso cref="MinOdorDeliveryTimeUS"/>
    /// <seealso cref="MaxOdorDeliveryTimeUS"/>
    /// <seealso cref="MinimumPokeTimeUS"/>
    /// <seealso cref="OdorDwellTimeUS"/>
    /// <seealso cref="Cam0PinState"/>
    /// <seealso cref="Cam0FrameRate"/>
    /// <seealso cref="Cam0DutyCycle"/>
    /// <seealso cref="EnableCam0Trigger"/>
    /// <seealso cref="Cam1PinState"/>
    /// <seealso cref="Cam1FrameRate"/>
    /// <seealso cref="Cam1DutyCycle"/>
    /// <seealso cref="EnableCam1Trigger"/>
    /// <seealso cref="EnableValveLeds"/>
    /// <seealso cref="LatestFlowRate"/>
    /// <seealso cref="LatestRawAdcSample"/>
    /// <seealso cref="EnableAdcSampling"/>
    /// <seealso cref="LeakAdcChannel"/>
    /// <seealso cref="LeakThreshold"/>
    /// <seealso cref="LeakState"/>
    /// <seealso cref="ManualFlowMeter"/>
    /// <seealso cref="NominalFlowRate"/>
    /// <seealso cref="FlowRateTolerance"/>
    /// <seealso cref="ManualFlowMeterState"/>
    /// <seealso cref="FlowMeterCalibrations"/>
    /// <seealso cref="PidUpdateFrequency"/>
    /// <seealso cref="PidGains"/>
    /// <seealso cref="ProportionalValve0Adc"/>
    /// <seealso cref="ProportionalValve0EnablePid"/>
    /// <seealso cref="ProportionalValve0DutyCycle"/>
    /// <seealso cref="ProportionalValve0TargetFlowRate"/>
    /// <seealso cref="ProportionalValve1Adc"/>
    /// <seealso cref="ProportionalValve1EnablePid"/>
    /// <seealso cref="ProportionalValve1DutyCycle"/>
    /// <seealso cref="ProportionalValve1TargetFlowRate"/>
    /// <seealso cref="ProportionalValve2Adc"/>
    /// <seealso cref="ProportionalValve2EnablePid"/>
    /// <seealso cref="ProportionalValve2DutyCycle"/>
    /// <seealso cref="ProportionalValve2TargetFlowRate"/>
    /// <seealso cref="FreezePidUpdates"/>
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
    [XmlInclude(typeof(QueuedOdorMask))]
    [XmlInclude(typeof(OdorSetupTimeUS))]
    [XmlInclude(typeof(MinOdorDeliveryTimeUS))]
    [XmlInclude(typeof(MaxOdorDeliveryTimeUS))]
    [XmlInclude(typeof(MinimumPokeTimeUS))]
    [XmlInclude(typeof(OdorDwellTimeUS))]
    [XmlInclude(typeof(Cam0PinState))]
    [XmlInclude(typeof(Cam0FrameRate))]
    [XmlInclude(typeof(Cam0DutyCycle))]
    [XmlInclude(typeof(EnableCam0Trigger))]
    [XmlInclude(typeof(Cam1PinState))]
    [XmlInclude(typeof(Cam1FrameRate))]
    [XmlInclude(typeof(Cam1DutyCycle))]
    [XmlInclude(typeof(EnableCam1Trigger))]
    [XmlInclude(typeof(EnableValveLeds))]
    [XmlInclude(typeof(LatestFlowRate))]
    [XmlInclude(typeof(LatestRawAdcSample))]
    [XmlInclude(typeof(EnableAdcSampling))]
    [XmlInclude(typeof(LeakAdcChannel))]
    [XmlInclude(typeof(LeakThreshold))]
    [XmlInclude(typeof(LeakState))]
    [XmlInclude(typeof(ManualFlowMeter))]
    [XmlInclude(typeof(NominalFlowRate))]
    [XmlInclude(typeof(FlowRateTolerance))]
    [XmlInclude(typeof(ManualFlowMeterState))]
    [XmlInclude(typeof(FlowMeterCalibrations))]
    [XmlInclude(typeof(PidUpdateFrequency))]
    [XmlInclude(typeof(PidGains))]
    [XmlInclude(typeof(ProportionalValve0Adc))]
    [XmlInclude(typeof(ProportionalValve0EnablePid))]
    [XmlInclude(typeof(ProportionalValve0DutyCycle))]
    [XmlInclude(typeof(ProportionalValve0TargetFlowRate))]
    [XmlInclude(typeof(ProportionalValve1Adc))]
    [XmlInclude(typeof(ProportionalValve1EnablePid))]
    [XmlInclude(typeof(ProportionalValve1DutyCycle))]
    [XmlInclude(typeof(ProportionalValve1TargetFlowRate))]
    [XmlInclude(typeof(ProportionalValve2Adc))]
    [XmlInclude(typeof(ProportionalValve2EnablePid))]
    [XmlInclude(typeof(ProportionalValve2DutyCycle))]
    [XmlInclude(typeof(ProportionalValve2TargetFlowRate))]
    [XmlInclude(typeof(FreezePidUpdates))]
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
    /// <seealso cref="QueuedOdorMask"/>
    /// <seealso cref="OdorSetupTimeUS"/>
    /// <seealso cref="MinOdorDeliveryTimeUS"/>
    /// <seealso cref="MaxOdorDeliveryTimeUS"/>
    /// <seealso cref="MinimumPokeTimeUS"/>
    /// <seealso cref="OdorDwellTimeUS"/>
    /// <seealso cref="Cam0PinState"/>
    /// <seealso cref="Cam0FrameRate"/>
    /// <seealso cref="Cam0DutyCycle"/>
    /// <seealso cref="EnableCam0Trigger"/>
    /// <seealso cref="Cam1PinState"/>
    /// <seealso cref="Cam1FrameRate"/>
    /// <seealso cref="Cam1DutyCycle"/>
    /// <seealso cref="EnableCam1Trigger"/>
    /// <seealso cref="EnableValveLeds"/>
    /// <seealso cref="LatestFlowRate"/>
    /// <seealso cref="LatestRawAdcSample"/>
    /// <seealso cref="EnableAdcSampling"/>
    /// <seealso cref="LeakAdcChannel"/>
    /// <seealso cref="LeakThreshold"/>
    /// <seealso cref="LeakState"/>
    /// <seealso cref="ManualFlowMeter"/>
    /// <seealso cref="NominalFlowRate"/>
    /// <seealso cref="FlowRateTolerance"/>
    /// <seealso cref="ManualFlowMeterState"/>
    /// <seealso cref="FlowMeterCalibrations"/>
    /// <seealso cref="PidUpdateFrequency"/>
    /// <seealso cref="PidGains"/>
    /// <seealso cref="ProportionalValve0Adc"/>
    /// <seealso cref="ProportionalValve0EnablePid"/>
    /// <seealso cref="ProportionalValve0DutyCycle"/>
    /// <seealso cref="ProportionalValve0TargetFlowRate"/>
    /// <seealso cref="ProportionalValve1Adc"/>
    /// <seealso cref="ProportionalValve1EnablePid"/>
    /// <seealso cref="ProportionalValve1DutyCycle"/>
    /// <seealso cref="ProportionalValve1TargetFlowRate"/>
    /// <seealso cref="ProportionalValve2Adc"/>
    /// <seealso cref="ProportionalValve2EnablePid"/>
    /// <seealso cref="ProportionalValve2DutyCycle"/>
    /// <seealso cref="ProportionalValve2TargetFlowRate"/>
    /// <seealso cref="FreezePidUpdates"/>
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
    [XmlInclude(typeof(QueuedOdorMask))]
    [XmlInclude(typeof(OdorSetupTimeUS))]
    [XmlInclude(typeof(MinOdorDeliveryTimeUS))]
    [XmlInclude(typeof(MaxOdorDeliveryTimeUS))]
    [XmlInclude(typeof(MinimumPokeTimeUS))]
    [XmlInclude(typeof(OdorDwellTimeUS))]
    [XmlInclude(typeof(Cam0PinState))]
    [XmlInclude(typeof(Cam0FrameRate))]
    [XmlInclude(typeof(Cam0DutyCycle))]
    [XmlInclude(typeof(EnableCam0Trigger))]
    [XmlInclude(typeof(Cam1PinState))]
    [XmlInclude(typeof(Cam1FrameRate))]
    [XmlInclude(typeof(Cam1DutyCycle))]
    [XmlInclude(typeof(EnableCam1Trigger))]
    [XmlInclude(typeof(EnableValveLeds))]
    [XmlInclude(typeof(LatestFlowRate))]
    [XmlInclude(typeof(LatestRawAdcSample))]
    [XmlInclude(typeof(EnableAdcSampling))]
    [XmlInclude(typeof(LeakAdcChannel))]
    [XmlInclude(typeof(LeakThreshold))]
    [XmlInclude(typeof(LeakState))]
    [XmlInclude(typeof(ManualFlowMeter))]
    [XmlInclude(typeof(NominalFlowRate))]
    [XmlInclude(typeof(FlowRateTolerance))]
    [XmlInclude(typeof(ManualFlowMeterState))]
    [XmlInclude(typeof(FlowMeterCalibrations))]
    [XmlInclude(typeof(PidUpdateFrequency))]
    [XmlInclude(typeof(PidGains))]
    [XmlInclude(typeof(ProportionalValve0Adc))]
    [XmlInclude(typeof(ProportionalValve0EnablePid))]
    [XmlInclude(typeof(ProportionalValve0DutyCycle))]
    [XmlInclude(typeof(ProportionalValve0TargetFlowRate))]
    [XmlInclude(typeof(ProportionalValve1Adc))]
    [XmlInclude(typeof(ProportionalValve1EnablePid))]
    [XmlInclude(typeof(ProportionalValve1DutyCycle))]
    [XmlInclude(typeof(ProportionalValve1TargetFlowRate))]
    [XmlInclude(typeof(ProportionalValve2Adc))]
    [XmlInclude(typeof(ProportionalValve2EnablePid))]
    [XmlInclude(typeof(ProportionalValve2DutyCycle))]
    [XmlInclude(typeof(ProportionalValve2TargetFlowRate))]
    [XmlInclude(typeof(FreezePidUpdates))]
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
    [XmlInclude(typeof(TimestampedQueuedOdorMask))]
    [XmlInclude(typeof(TimestampedOdorSetupTimeUS))]
    [XmlInclude(typeof(TimestampedMinOdorDeliveryTimeUS))]
    [XmlInclude(typeof(TimestampedMaxOdorDeliveryTimeUS))]
    [XmlInclude(typeof(TimestampedMinimumPokeTimeUS))]
    [XmlInclude(typeof(TimestampedOdorDwellTimeUS))]
    [XmlInclude(typeof(TimestampedCam0PinState))]
    [XmlInclude(typeof(TimestampedCam0FrameRate))]
    [XmlInclude(typeof(TimestampedCam0DutyCycle))]
    [XmlInclude(typeof(TimestampedEnableCam0Trigger))]
    [XmlInclude(typeof(TimestampedCam1PinState))]
    [XmlInclude(typeof(TimestampedCam1FrameRate))]
    [XmlInclude(typeof(TimestampedCam1DutyCycle))]
    [XmlInclude(typeof(TimestampedEnableCam1Trigger))]
    [XmlInclude(typeof(TimestampedEnableValveLeds))]
    [XmlInclude(typeof(TimestampedLatestFlowRate))]
    [XmlInclude(typeof(TimestampedLatestRawAdcSample))]
    [XmlInclude(typeof(TimestampedEnableAdcSampling))]
    [XmlInclude(typeof(TimestampedLeakAdcChannel))]
    [XmlInclude(typeof(TimestampedLeakThreshold))]
    [XmlInclude(typeof(TimestampedLeakState))]
    [XmlInclude(typeof(TimestampedManualFlowMeter))]
    [XmlInclude(typeof(TimestampedNominalFlowRate))]
    [XmlInclude(typeof(TimestampedFlowRateTolerance))]
    [XmlInclude(typeof(TimestampedManualFlowMeterState))]
    [XmlInclude(typeof(TimestampedFlowMeterCalibrations))]
    [XmlInclude(typeof(TimestampedPidUpdateFrequency))]
    [XmlInclude(typeof(TimestampedPidGains))]
    [XmlInclude(typeof(TimestampedProportionalValve0Adc))]
    [XmlInclude(typeof(TimestampedProportionalValve0EnablePid))]
    [XmlInclude(typeof(TimestampedProportionalValve0DutyCycle))]
    [XmlInclude(typeof(TimestampedProportionalValve0TargetFlowRate))]
    [XmlInclude(typeof(TimestampedProportionalValve1Adc))]
    [XmlInclude(typeof(TimestampedProportionalValve1EnablePid))]
    [XmlInclude(typeof(TimestampedProportionalValve1DutyCycle))]
    [XmlInclude(typeof(TimestampedProportionalValve1TargetFlowRate))]
    [XmlInclude(typeof(TimestampedProportionalValve2Adc))]
    [XmlInclude(typeof(TimestampedProportionalValve2EnablePid))]
    [XmlInclude(typeof(TimestampedProportionalValve2DutyCycle))]
    [XmlInclude(typeof(TimestampedProportionalValve2TargetFlowRate))]
    [XmlInclude(typeof(TimestampedFreezePidUpdates))]
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
    /// <seealso cref="QueuedOdorMask"/>
    /// <seealso cref="OdorSetupTimeUS"/>
    /// <seealso cref="MinOdorDeliveryTimeUS"/>
    /// <seealso cref="MaxOdorDeliveryTimeUS"/>
    /// <seealso cref="MinimumPokeTimeUS"/>
    /// <seealso cref="OdorDwellTimeUS"/>
    /// <seealso cref="Cam0PinState"/>
    /// <seealso cref="Cam0FrameRate"/>
    /// <seealso cref="Cam0DutyCycle"/>
    /// <seealso cref="EnableCam0Trigger"/>
    /// <seealso cref="Cam1PinState"/>
    /// <seealso cref="Cam1FrameRate"/>
    /// <seealso cref="Cam1DutyCycle"/>
    /// <seealso cref="EnableCam1Trigger"/>
    /// <seealso cref="EnableValveLeds"/>
    /// <seealso cref="LatestFlowRate"/>
    /// <seealso cref="LatestRawAdcSample"/>
    /// <seealso cref="EnableAdcSampling"/>
    /// <seealso cref="LeakAdcChannel"/>
    /// <seealso cref="LeakThreshold"/>
    /// <seealso cref="LeakState"/>
    /// <seealso cref="ManualFlowMeter"/>
    /// <seealso cref="NominalFlowRate"/>
    /// <seealso cref="FlowRateTolerance"/>
    /// <seealso cref="ManualFlowMeterState"/>
    /// <seealso cref="FlowMeterCalibrations"/>
    /// <seealso cref="PidUpdateFrequency"/>
    /// <seealso cref="PidGains"/>
    /// <seealso cref="ProportionalValve0Adc"/>
    /// <seealso cref="ProportionalValve0EnablePid"/>
    /// <seealso cref="ProportionalValve0DutyCycle"/>
    /// <seealso cref="ProportionalValve0TargetFlowRate"/>
    /// <seealso cref="ProportionalValve1Adc"/>
    /// <seealso cref="ProportionalValve1EnablePid"/>
    /// <seealso cref="ProportionalValve1DutyCycle"/>
    /// <seealso cref="ProportionalValve1TargetFlowRate"/>
    /// <seealso cref="ProportionalValve2Adc"/>
    /// <seealso cref="ProportionalValve2EnablePid"/>
    /// <seealso cref="ProportionalValve2DutyCycle"/>
    /// <seealso cref="ProportionalValve2TargetFlowRate"/>
    /// <seealso cref="FreezePidUpdates"/>
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
    [XmlInclude(typeof(QueuedOdorMask))]
    [XmlInclude(typeof(OdorSetupTimeUS))]
    [XmlInclude(typeof(MinOdorDeliveryTimeUS))]
    [XmlInclude(typeof(MaxOdorDeliveryTimeUS))]
    [XmlInclude(typeof(MinimumPokeTimeUS))]
    [XmlInclude(typeof(OdorDwellTimeUS))]
    [XmlInclude(typeof(Cam0PinState))]
    [XmlInclude(typeof(Cam0FrameRate))]
    [XmlInclude(typeof(Cam0DutyCycle))]
    [XmlInclude(typeof(EnableCam0Trigger))]
    [XmlInclude(typeof(Cam1PinState))]
    [XmlInclude(typeof(Cam1FrameRate))]
    [XmlInclude(typeof(Cam1DutyCycle))]
    [XmlInclude(typeof(EnableCam1Trigger))]
    [XmlInclude(typeof(EnableValveLeds))]
    [XmlInclude(typeof(LatestFlowRate))]
    [XmlInclude(typeof(LatestRawAdcSample))]
    [XmlInclude(typeof(EnableAdcSampling))]
    [XmlInclude(typeof(LeakAdcChannel))]
    [XmlInclude(typeof(LeakThreshold))]
    [XmlInclude(typeof(LeakState))]
    [XmlInclude(typeof(ManualFlowMeter))]
    [XmlInclude(typeof(NominalFlowRate))]
    [XmlInclude(typeof(FlowRateTolerance))]
    [XmlInclude(typeof(ManualFlowMeterState))]
    [XmlInclude(typeof(FlowMeterCalibrations))]
    [XmlInclude(typeof(PidUpdateFrequency))]
    [XmlInclude(typeof(PidGains))]
    [XmlInclude(typeof(ProportionalValve0Adc))]
    [XmlInclude(typeof(ProportionalValve0EnablePid))]
    [XmlInclude(typeof(ProportionalValve0DutyCycle))]
    [XmlInclude(typeof(ProportionalValve0TargetFlowRate))]
    [XmlInclude(typeof(ProportionalValve1Adc))]
    [XmlInclude(typeof(ProportionalValve1EnablePid))]
    [XmlInclude(typeof(ProportionalValve1DutyCycle))]
    [XmlInclude(typeof(ProportionalValve1TargetFlowRate))]
    [XmlInclude(typeof(ProportionalValve2Adc))]
    [XmlInclude(typeof(ProportionalValve2EnablePid))]
    [XmlInclude(typeof(ProportionalValve2DutyCycle))]
    [XmlInclude(typeof(ProportionalValve2TargetFlowRate))]
    [XmlInclude(typeof(FreezePidUpdates))]
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
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ValveState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
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
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
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
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
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
        public static Timestamped<ushort> GetPayload(HarpMessage message)
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
    /// Represents a register that set the state (on or off) of any auxiliary GPIO pins specified as outputs.
    /// </summary>
    [Description("Set the state (on or off) of any auxiliary GPIO pins specified as outputs.")]
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
    /// Represents a register that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
    /// </summary>
    [Description("Enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.")]
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
    /// Represents a register that queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed.
    /// </summary>
    [Description("Queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed")]
    public partial class QueuedOdorMask
    {
        /// <summary>
        /// Represents the address of the <see cref="QueuedOdorMask"/> register. This field is constant.
        /// </summary>
        public const int Address = 66;

        /// <summary>
        /// Represents the payload type of the <see cref="QueuedOdorMask"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="QueuedOdorMask"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="QueuedOdorMask"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="QueuedOdorMask"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="QueuedOdorMask"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="QueuedOdorMask"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="QueuedOdorMask"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="QueuedOdorMask"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// QueuedOdorMask register.
    /// </summary>
    /// <seealso cref="QueuedOdorMask"/>
    [Description("Filters and selects timestamped messages from the QueuedOdorMask register.")]
    public partial class TimestampedQueuedOdorMask
    {
        /// <summary>
        /// Represents the address of the <see cref="QueuedOdorMask"/> register. This field is constant.
        /// </summary>
        public const int Address = QueuedOdorMask.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="QueuedOdorMask"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return QueuedOdorMask.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time alotted (in microseconds) for the vacuum valve to close.
    /// </summary>
    [Description("Time alotted (in microseconds) for the vacuum valve to close.")]
    public partial class OdorSetupTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="OdorSetupTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 67;

        /// <summary>
        /// Represents the payload type of the <see cref="OdorSetupTimeUS"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="OdorSetupTimeUS"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OdorSetupTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OdorSetupTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OdorSetupTimeUS"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OdorSetupTimeUS"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OdorSetupTimeUS"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OdorSetupTimeUS"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OdorSetupTimeUS register.
    /// </summary>
    /// <seealso cref="OdorSetupTimeUS"/>
    [Description("Filters and selects timestamped messages from the OdorSetupTimeUS register.")]
    public partial class TimestampedOdorSetupTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="OdorSetupTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = OdorSetupTimeUS.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OdorSetupTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return OdorSetupTimeUS.GetTimestampedPayload(message);
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
    /// Represents a register that minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.
    /// </summary>
    [Description("Minimum time (in microseconds) necessary for a mouse poke port beam to be broken before being interpretted as a poke.")]
    public partial class MinimumPokeTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="MinimumPokeTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 70;

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
    /// Represents a register that time (in microseconds) that the odor remains in the delivery state.
    /// </summary>
    [Description("Time (in microseconds) that the odor remains in the delivery state.")]
    public partial class OdorDwellTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="OdorDwellTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = 71;

        /// <summary>
        /// Represents the payload type of the <see cref="OdorDwellTimeUS"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="OdorDwellTimeUS"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OdorDwellTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OdorDwellTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OdorDwellTimeUS"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OdorDwellTimeUS"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OdorDwellTimeUS"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OdorDwellTimeUS"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OdorDwellTimeUS register.
    /// </summary>
    /// <seealso cref="OdorDwellTimeUS"/>
    [Description("Filters and selects timestamped messages from the OdorDwellTimeUS register.")]
    public partial class TimestampedOdorDwellTimeUS
    {
        /// <summary>
        /// Represents the address of the <see cref="OdorDwellTimeUS"/> register. This field is constant.
        /// </summary>
        public const int Address = OdorDwellTimeUS.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OdorDwellTimeUS"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return OdorDwellTimeUS.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
    /// </summary>
    [Description("Event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
    public partial class Cam0PinState
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam0PinState"/> register. This field is constant.
        /// </summary>
        public const int Address = 72;

        /// <summary>
        /// Represents the payload type of the <see cref="Cam0PinState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Cam0PinState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Cam0PinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Cam0PinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Cam0PinState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam0PinState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Cam0PinState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam0PinState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Cam0PinState register.
    /// </summary>
    /// <seealso cref="Cam0PinState"/>
    [Description("Filters and selects timestamped messages from the Cam0PinState register.")]
    public partial class TimestampedCam0PinState
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam0PinState"/> register. This field is constant.
        /// </summary>
        public const int Address = Cam0PinState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Cam0PinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return Cam0PinState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the frame rate of the camera trigger/ frequency of the PWM signal.
    /// </summary>
    [Description("Set the frame rate of the camera trigger/ frequency of the PWM signal.")]
    public partial class Cam0FrameRate
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam0FrameRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 73;

        /// <summary>
        /// Represents the payload type of the <see cref="Cam0FrameRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="Cam0FrameRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Cam0FrameRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Cam0FrameRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Cam0FrameRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam0FrameRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Cam0FrameRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam0FrameRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Cam0FrameRate register.
    /// </summary>
    /// <seealso cref="Cam0FrameRate"/>
    [Description("Filters and selects timestamped messages from the Cam0FrameRate register.")]
    public partial class TimestampedCam0FrameRate
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam0FrameRate"/> register. This field is constant.
        /// </summary>
        public const int Address = Cam0FrameRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Cam0FrameRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return Cam0FrameRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
    /// </summary>
    [Description("Set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
    public partial class Cam0DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam0DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = 74;

        /// <summary>
        /// Represents the payload type of the <see cref="Cam0DutyCycle"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Cam0DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Cam0DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Cam0DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Cam0DutyCycle"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam0DutyCycle"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Cam0DutyCycle"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam0DutyCycle"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Cam0DutyCycle register.
    /// </summary>
    /// <seealso cref="Cam0DutyCycle"/>
    [Description("Filters and selects timestamped messages from the Cam0DutyCycle register.")]
    public partial class TimestampedCam0DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam0DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = Cam0DutyCycle.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Cam0DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Cam0DutyCycle.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable (1) and disable (0) camera triggering/ the PWM signal.
    /// </summary>
    [Description("Enable (1) and disable (0) camera triggering/ the PWM signal.")]
    public partial class EnableCam0Trigger
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableCam0Trigger"/> register. This field is constant.
        /// </summary>
        public const int Address = 75;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableCam0Trigger"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableCam0Trigger"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableCam0Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableCam0Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableCam0Trigger"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableCam0Trigger"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableCam0Trigger"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableCam0Trigger"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableCam0Trigger register.
    /// </summary>
    /// <seealso cref="EnableCam0Trigger"/>
    [Description("Filters and selects timestamped messages from the EnableCam0Trigger register.")]
    public partial class TimestampedEnableCam0Trigger
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableCam0Trigger"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableCam0Trigger.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableCam0Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return EnableCam0Trigger.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
    /// </summary>
    [Description("Event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
    public partial class Cam1PinState
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam1PinState"/> register. This field is constant.
        /// </summary>
        public const int Address = 76;

        /// <summary>
        /// Represents the payload type of the <see cref="Cam1PinState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Cam1PinState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Cam1PinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Cam1PinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Cam1PinState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam1PinState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Cam1PinState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam1PinState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Cam1PinState register.
    /// </summary>
    /// <seealso cref="Cam1PinState"/>
    [Description("Filters and selects timestamped messages from the Cam1PinState register.")]
    public partial class TimestampedCam1PinState
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam1PinState"/> register. This field is constant.
        /// </summary>
        public const int Address = Cam1PinState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Cam1PinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return Cam1PinState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the frame rate of the camera trigger/ frequency of the PWM signal.
    /// </summary>
    [Description("Set the frame rate of the camera trigger/ frequency of the PWM signal.")]
    public partial class Cam1FrameRate
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam1FrameRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 77;

        /// <summary>
        /// Represents the payload type of the <see cref="Cam1FrameRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="Cam1FrameRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Cam1FrameRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Cam1FrameRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Cam1FrameRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam1FrameRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Cam1FrameRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam1FrameRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Cam1FrameRate register.
    /// </summary>
    /// <seealso cref="Cam1FrameRate"/>
    [Description("Filters and selects timestamped messages from the Cam1FrameRate register.")]
    public partial class TimestampedCam1FrameRate
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam1FrameRate"/> register. This field is constant.
        /// </summary>
        public const int Address = Cam1FrameRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Cam1FrameRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return Cam1FrameRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
    /// </summary>
    [Description("Set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
    public partial class Cam1DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam1DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = 78;

        /// <summary>
        /// Represents the payload type of the <see cref="Cam1DutyCycle"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Cam1DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Cam1DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Cam1DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Cam1DutyCycle"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam1DutyCycle"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Cam1DutyCycle"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Cam1DutyCycle"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Cam1DutyCycle register.
    /// </summary>
    /// <seealso cref="Cam1DutyCycle"/>
    [Description("Filters and selects timestamped messages from the Cam1DutyCycle register.")]
    public partial class TimestampedCam1DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="Cam1DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = Cam1DutyCycle.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Cam1DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Cam1DutyCycle.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable (1) and disable (0) camera triggering/ the PWM signal.
    /// </summary>
    [Description("Enable (1) and disable (0) camera triggering/ the PWM signal.")]
    public partial class EnableCam1Trigger
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableCam1Trigger"/> register. This field is constant.
        /// </summary>
        public const int Address = 79;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableCam1Trigger"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableCam1Trigger"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableCam1Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableCam1Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableCam1Trigger"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableCam1Trigger"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableCam1Trigger"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableCam1Trigger"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableCam1Trigger register.
    /// </summary>
    /// <seealso cref="EnableCam1Trigger"/>
    [Description("Filters and selects timestamped messages from the EnableCam1Trigger register.")]
    public partial class TimestampedEnableCam1Trigger
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableCam1Trigger"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableCam1Trigger.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableCam1Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return EnableCam1Trigger.GetTimestampedPayload(message);
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
        public const int Address = 80;

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
    /// Represents a register that latest flow rate measurement sample from ADC0-8.
    /// </summary>
    [Description("Latest flow rate measurement sample from ADC0-8.")]
    public partial class LatestFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="LatestFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 81;

        /// <summary>
        /// Represents the payload type of the <see cref="LatestFlowRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="LatestFlowRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 8;

        static LatestFlowRatePayload ParsePayload(float[] payload)
        {
            LatestFlowRatePayload result;
            result.ADC0 = payload[0];
            result.ADC1 = payload[1];
            result.ADC2 = payload[2];
            result.ADC3 = payload[3];
            result.ADC4 = payload[4];
            result.ADC5 = payload[5];
            result.ADC6 = payload[6];
            result.ADC7 = payload[7];
            return result;
        }

        static float[] FormatPayload(LatestFlowRatePayload value)
        {
            float[] result;
            result = new float[8];
            result[0] = value.ADC0;
            result[1] = value.ADC1;
            result[2] = value.ADC2;
            result[3] = value.ADC3;
            result[4] = value.ADC4;
            result[5] = value.ADC5;
            result[6] = value.ADC6;
            result[7] = value.ADC7;
            return result;
        }

        /// <summary>
        /// Returns the payload data for <see cref="LatestFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LatestFlowRatePayload GetPayload(HarpMessage message)
        {
            return ParsePayload(message.GetPayloadArray<float>());
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LatestFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LatestFlowRatePayload> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadArray<float>();
            return Timestamped.Create(ParsePayload(payload.Value), payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LatestFlowRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LatestFlowRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LatestFlowRatePayload value)
        {
            return HarpMessage.FromSingle(Address, messageType, FormatPayload(value));
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LatestFlowRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LatestFlowRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LatestFlowRatePayload value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, FormatPayload(value));
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LatestFlowRate register.
    /// </summary>
    /// <seealso cref="LatestFlowRate"/>
    [Description("Filters and selects timestamped messages from the LatestFlowRate register.")]
    public partial class TimestampedLatestFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="LatestFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = LatestFlowRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LatestFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LatestFlowRatePayload> GetPayload(HarpMessage message)
        {
            return LatestFlowRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that latest raw bit measurement sample from ADC0-8.
    /// </summary>
    [Description("Latest raw bit measurement sample from ADC0-8.")]
    public partial class LatestRawAdcSample
    {
        /// <summary>
        /// Represents the address of the <see cref="LatestRawAdcSample"/> register. This field is constant.
        /// </summary>
        public const int Address = 82;

        /// <summary>
        /// Represents the payload type of the <see cref="LatestRawAdcSample"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="LatestRawAdcSample"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 8;

        static LatestRawAdcSamplePayload ParsePayload(float[] payload)
        {
            LatestRawAdcSamplePayload result;
            result.ADC0 = payload[0];
            result.ADC1 = payload[1];
            result.ADC2 = payload[2];
            result.ADC3 = payload[3];
            result.ADC4 = payload[4];
            result.ADC5 = payload[5];
            result.ADC6 = payload[6];
            result.ADC7 = payload[7];
            return result;
        }

        static float[] FormatPayload(LatestRawAdcSamplePayload value)
        {
            float[] result;
            result = new float[8];
            result[0] = value.ADC0;
            result[1] = value.ADC1;
            result[2] = value.ADC2;
            result[3] = value.ADC3;
            result[4] = value.ADC4;
            result[5] = value.ADC5;
            result[6] = value.ADC6;
            result[7] = value.ADC7;
            return result;
        }

        /// <summary>
        /// Returns the payload data for <see cref="LatestRawAdcSample"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LatestRawAdcSamplePayload GetPayload(HarpMessage message)
        {
            return ParsePayload(message.GetPayloadArray<float>());
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LatestRawAdcSample"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LatestRawAdcSamplePayload> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadArray<float>();
            return Timestamped.Create(ParsePayload(payload.Value), payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LatestRawAdcSample"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LatestRawAdcSample"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LatestRawAdcSamplePayload value)
        {
            return HarpMessage.FromSingle(Address, messageType, FormatPayload(value));
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LatestRawAdcSample"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LatestRawAdcSample"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LatestRawAdcSamplePayload value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, FormatPayload(value));
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LatestRawAdcSample register.
    /// </summary>
    /// <seealso cref="LatestRawAdcSample"/>
    [Description("Filters and selects timestamped messages from the LatestRawAdcSample register.")]
    public partial class TimestampedLatestRawAdcSample
    {
        /// <summary>
        /// Represents the address of the <see cref="LatestRawAdcSample"/> register. This field is constant.
        /// </summary>
        public const int Address = LatestRawAdcSample.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LatestRawAdcSample"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LatestRawAdcSamplePayload> GetPayload(HarpMessage message)
        {
            return LatestRawAdcSample.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable (1) and disable (0) ADC sampling.
    /// </summary>
    [Description("Enable (1) and disable (0) ADC sampling.")]
    public partial class EnableAdcSampling
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableAdcSampling"/> register. This field is constant.
        /// </summary>
        public const int Address = 83;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableAdcSampling"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableAdcSampling"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableAdcSampling"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableAdcSampling"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableAdcSampling"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableAdcSampling"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableAdcSampling"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableAdcSampling"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableAdcSampling register.
    /// </summary>
    /// <seealso cref="EnableAdcSampling"/>
    [Description("Filters and selects timestamped messages from the EnableAdcSampling register.")]
    public partial class TimestampedEnableAdcSampling
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableAdcSampling"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableAdcSampling.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableAdcSampling"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return EnableAdcSampling.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the ADC channel for leak detection. Leak detection is off by Default (-1).
    /// </summary>
    [Description("Set the ADC channel for leak detection. Leak detection is off by Default (-1).")]
    public partial class LeakAdcChannel
    {
        /// <summary>
        /// Represents the address of the <see cref="LeakAdcChannel"/> register. This field is constant.
        /// </summary>
        public const int Address = 84;

        /// <summary>
        /// Represents the payload type of the <see cref="LeakAdcChannel"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.S8;

        /// <summary>
        /// Represents the length of the <see cref="LeakAdcChannel"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LeakAdcChannel"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static sbyte GetPayload(HarpMessage message)
        {
            return message.GetPayloadSByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LeakAdcChannel"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<sbyte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LeakAdcChannel"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LeakAdcChannel"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, sbyte value)
        {
            return HarpMessage.FromSByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LeakAdcChannel"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LeakAdcChannel"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, sbyte value)
        {
            return HarpMessage.FromSByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LeakAdcChannel register.
    /// </summary>
    /// <seealso cref="LeakAdcChannel"/>
    [Description("Filters and selects timestamped messages from the LeakAdcChannel register.")]
    public partial class TimestampedLeakAdcChannel
    {
        /// <summary>
        /// Represents the address of the <see cref="LeakAdcChannel"/> register. This field is constant.
        /// </summary>
        public const int Address = LeakAdcChannel.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LeakAdcChannel"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<sbyte> GetPayload(HarpMessage message)
        {
            return LeakAdcChannel.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the threshold for leak detection in mL/min.
    /// </summary>
    [Description("Set the threshold for leak detection in mL/min.")]
    public partial class LeakThreshold
    {
        /// <summary>
        /// Represents the address of the <see cref="LeakThreshold"/> register. This field is constant.
        /// </summary>
        public const int Address = 85;

        /// <summary>
        /// Represents the payload type of the <see cref="LeakThreshold"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="LeakThreshold"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LeakThreshold"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LeakThreshold"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LeakThreshold"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LeakThreshold"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LeakThreshold"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LeakThreshold"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LeakThreshold register.
    /// </summary>
    /// <seealso cref="LeakThreshold"/>
    [Description("Filters and selects timestamped messages from the LeakThreshold register.")]
    public partial class TimestampedLeakThreshold
    {
        /// <summary>
        /// Represents the address of the <see cref="LeakThreshold"/> register. This field is constant.
        /// </summary>
        public const int Address = LeakThreshold.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LeakThreshold"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return LeakThreshold.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the state for leak detection.
    /// </summary>
    [Description("Set the state for leak detection.")]
    public partial class LeakState
    {
        /// <summary>
        /// Represents the address of the <see cref="LeakState"/> register. This field is constant.
        /// </summary>
        public const int Address = 86;

        /// <summary>
        /// Represents the payload type of the <see cref="LeakState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="LeakState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LeakState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LeakState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LeakState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LeakState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LeakState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LeakState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LeakState register.
    /// </summary>
    /// <seealso cref="LeakState"/>
    [Description("Filters and selects timestamped messages from the LeakState register.")]
    public partial class TimestampedLeakState
    {
        /// <summary>
        /// Represents the address of the <see cref="LeakState"/> register. This field is constant.
        /// </summary>
        public const int Address = LeakState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LeakState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return LeakState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.
    /// </summary>
    [Description("Set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.")]
    public partial class ManualFlowMeter
    {
        /// <summary>
        /// Represents the address of the <see cref="ManualFlowMeter"/> register. This field is constant.
        /// </summary>
        public const int Address = 87;

        /// <summary>
        /// Represents the payload type of the <see cref="ManualFlowMeter"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.S8;

        /// <summary>
        /// Represents the length of the <see cref="ManualFlowMeter"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ManualFlowMeter"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static sbyte GetPayload(HarpMessage message)
        {
            return message.GetPayloadSByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ManualFlowMeter"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<sbyte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ManualFlowMeter"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ManualFlowMeter"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, sbyte value)
        {
            return HarpMessage.FromSByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ManualFlowMeter"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ManualFlowMeter"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, sbyte value)
        {
            return HarpMessage.FromSByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ManualFlowMeter register.
    /// </summary>
    /// <seealso cref="ManualFlowMeter"/>
    [Description("Filters and selects timestamped messages from the ManualFlowMeter register.")]
    public partial class TimestampedManualFlowMeter
    {
        /// <summary>
        /// Represents the address of the <see cref="ManualFlowMeter"/> register. This field is constant.
        /// </summary>
        public const int Address = ManualFlowMeter.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ManualFlowMeter"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<sbyte> GetPayload(HarpMessage message)
        {
            return ManualFlowMeter.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the nominal flow rate for manual flow meter calibration in mL/min.
    /// </summary>
    [Description("Set the nominal flow rate for manual flow meter calibration in mL/min.")]
    public partial class NominalFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="NominalFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 88;

        /// <summary>
        /// Represents the payload type of the <see cref="NominalFlowRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="NominalFlowRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="NominalFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="NominalFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="NominalFlowRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="NominalFlowRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="NominalFlowRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="NominalFlowRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// NominalFlowRate register.
    /// </summary>
    /// <seealso cref="NominalFlowRate"/>
    [Description("Filters and selects timestamped messages from the NominalFlowRate register.")]
    public partial class TimestampedNominalFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="NominalFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = NominalFlowRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="NominalFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return NominalFlowRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the tolerance for flow rate detection (e.g., +-0.1 mL/min).
    /// </summary>
    [Description("Set the tolerance for flow rate detection (e.g., +-0.1 mL/min).")]
    public partial class FlowRateTolerance
    {
        /// <summary>
        /// Represents the address of the <see cref="FlowRateTolerance"/> register. This field is constant.
        /// </summary>
        public const int Address = 89;

        /// <summary>
        /// Represents the payload type of the <see cref="FlowRateTolerance"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="FlowRateTolerance"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="FlowRateTolerance"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="FlowRateTolerance"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="FlowRateTolerance"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FlowRateTolerance"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="FlowRateTolerance"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FlowRateTolerance"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// FlowRateTolerance register.
    /// </summary>
    /// <seealso cref="FlowRateTolerance"/>
    [Description("Filters and selects timestamped messages from the FlowRateTolerance register.")]
    public partial class TimestampedFlowRateTolerance
    {
        /// <summary>
        /// Represents the address of the <see cref="FlowRateTolerance"/> register. This field is constant.
        /// </summary>
        public const int Address = FlowRateTolerance.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="FlowRateTolerance"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return FlowRateTolerance.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the state for manual flow meter calibration.
    /// </summary>
    [Description("Set the state for manual flow meter calibration.")]
    public partial class ManualFlowMeterState
    {
        /// <summary>
        /// Represents the address of the <see cref="ManualFlowMeterState"/> register. This field is constant.
        /// </summary>
        public const int Address = 90;

        /// <summary>
        /// Represents the payload type of the <see cref="ManualFlowMeterState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ManualFlowMeterState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ManualFlowMeterState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ManualFlowMeterState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ManualFlowMeterState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ManualFlowMeterState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ManualFlowMeterState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ManualFlowMeterState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ManualFlowMeterState register.
    /// </summary>
    /// <seealso cref="ManualFlowMeterState"/>
    [Description("Filters and selects timestamped messages from the ManualFlowMeterState register.")]
    public partial class TimestampedManualFlowMeterState
    {
        /// <summary>
        /// Represents the address of the <see cref="ManualFlowMeterState"/> register. This field is constant.
        /// </summary>
        public const int Address = ManualFlowMeterState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ManualFlowMeterState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return ManualFlowMeterState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that calibration for the flow meters.
    /// </summary>
    [Description("Calibration for the flow meters.")]
    public partial class FlowMeterCalibrations
    {
        /// <summary>
        /// Represents the address of the <see cref="FlowMeterCalibrations"/> register. This field is constant.
        /// </summary>
        public const int Address = 91;

        /// <summary>
        /// Represents the payload type of the <see cref="FlowMeterCalibrations"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="FlowMeterCalibrations"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 6;

        static FlowMeterCalibrationsPayload ParsePayload(float[] payload)
        {
            FlowMeterCalibrationsPayload result;
            result.A0 = payload[0];
            result.A1 = payload[1];
            result.A2 = payload[2];
            result.A3 = payload[3];
            result.A4 = payload[4];
            result.A5 = payload[5];
            return result;
        }

        static float[] FormatPayload(FlowMeterCalibrationsPayload value)
        {
            float[] result;
            result = new float[6];
            result[0] = value.A0;
            result[1] = value.A1;
            result[2] = value.A2;
            result[3] = value.A3;
            result[4] = value.A4;
            result[5] = value.A5;
            return result;
        }

        /// <summary>
        /// Returns the payload data for <see cref="FlowMeterCalibrations"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static FlowMeterCalibrationsPayload GetPayload(HarpMessage message)
        {
            return ParsePayload(message.GetPayloadArray<float>());
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="FlowMeterCalibrations"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<FlowMeterCalibrationsPayload> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadArray<float>();
            return Timestamped.Create(ParsePayload(payload.Value), payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="FlowMeterCalibrations"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FlowMeterCalibrations"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, FlowMeterCalibrationsPayload value)
        {
            return HarpMessage.FromSingle(Address, messageType, FormatPayload(value));
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="FlowMeterCalibrations"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FlowMeterCalibrations"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, FlowMeterCalibrationsPayload value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, FormatPayload(value));
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// FlowMeterCalibrations register.
    /// </summary>
    /// <seealso cref="FlowMeterCalibrations"/>
    [Description("Filters and selects timestamped messages from the FlowMeterCalibrations register.")]
    public partial class TimestampedFlowMeterCalibrations
    {
        /// <summary>
        /// Represents the address of the <see cref="FlowMeterCalibrations"/> register. This field is constant.
        /// </summary>
        public const int Address = FlowMeterCalibrations.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="FlowMeterCalibrations"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<FlowMeterCalibrationsPayload> GetPayload(HarpMessage message)
        {
            return FlowMeterCalibrations.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the update frequency for the PID controller.
    /// </summary>
    [Description("Set the update frequency for the PID controller.")]
    public partial class PidUpdateFrequency
    {
        /// <summary>
        /// Represents the address of the <see cref="PidUpdateFrequency"/> register. This field is constant.
        /// </summary>
        public const int Address = 92;

        /// <summary>
        /// Represents the payload type of the <see cref="PidUpdateFrequency"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="PidUpdateFrequency"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PidUpdateFrequency"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PidUpdateFrequency"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PidUpdateFrequency"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PidUpdateFrequency"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PidUpdateFrequency"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PidUpdateFrequency"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PidUpdateFrequency register.
    /// </summary>
    /// <seealso cref="PidUpdateFrequency"/>
    [Description("Filters and selects timestamped messages from the PidUpdateFrequency register.")]
    public partial class TimestampedPidUpdateFrequency
    {
        /// <summary>
        /// Represents the address of the <see cref="PidUpdateFrequency"/> register. This field is constant.
        /// </summary>
        public const int Address = PidUpdateFrequency.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PidUpdateFrequency"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return PidUpdateFrequency.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the PID Kp, Ki, and Kd gains for the controller.
    /// </summary>
    [Description("Set the PID Kp, Ki, and Kd gains for the controller.")]
    public partial class PidGains
    {
        /// <summary>
        /// Represents the address of the <see cref="PidGains"/> register. This field is constant.
        /// </summary>
        public const int Address = 93;

        /// <summary>
        /// Represents the payload type of the <see cref="PidGains"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="PidGains"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 3;

        static PidGainsPayload ParsePayload(float[] payload)
        {
            PidGainsPayload result;
            result.Kp = payload[0];
            result.Ki = payload[1];
            result.Kd = payload[2];
            return result;
        }

        static float[] FormatPayload(PidGainsPayload value)
        {
            float[] result;
            result = new float[3];
            result[0] = value.Kp;
            result[1] = value.Ki;
            result[2] = value.Kd;
            return result;
        }

        /// <summary>
        /// Returns the payload data for <see cref="PidGains"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static PidGainsPayload GetPayload(HarpMessage message)
        {
            return ParsePayload(message.GetPayloadArray<float>());
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PidGains"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<PidGainsPayload> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadArray<float>();
            return Timestamped.Create(ParsePayload(payload.Value), payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PidGains"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PidGains"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, PidGainsPayload value)
        {
            return HarpMessage.FromSingle(Address, messageType, FormatPayload(value));
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PidGains"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PidGains"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, PidGainsPayload value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, FormatPayload(value));
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PidGains register.
    /// </summary>
    /// <seealso cref="PidGains"/>
    [Description("Filters and selects timestamped messages from the PidGains register.")]
    public partial class TimestampedPidGains
    {
        /// <summary>
        /// Represents the address of the <see cref="PidGains"/> register. This field is constant.
        /// </summary>
        public const int Address = PidGains.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PidGains"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<PidGainsPayload> GetPayload(HarpMessage message)
        {
            return PidGains.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the ADC channel used for the control of proportional valve 0.
    /// </summary>
    [Description("Set the ADC channel used for the control of proportional valve 0.")]
    public partial class ProportionalValve0Adc
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve0Adc"/> register. This field is constant.
        /// </summary>
        public const int Address = 94;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve0Adc"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve0Adc"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve0Adc"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve0Adc"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve0Adc"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve0Adc"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve0Adc"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve0Adc"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve0Adc register.
    /// </summary>
    /// <seealso cref="ProportionalValve0Adc"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve0Adc register.")]
    public partial class TimestampedProportionalValve0Adc
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve0Adc"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve0Adc.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve0Adc"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return ProportionalValve0Adc.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable (1) or disable (0) PID control for proportional valve 0.
    /// </summary>
    [Description("Enable (1) or disable (0) PID control for proportional valve 0.")]
    public partial class ProportionalValve0EnablePid
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve0EnablePid"/> register. This field is constant.
        /// </summary>
        public const int Address = 95;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve0EnablePid"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve0EnablePid"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve0EnablePid"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve0EnablePid"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve0EnablePid"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve0EnablePid"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve0EnablePid"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve0EnablePid"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve0EnablePid register.
    /// </summary>
    /// <seealso cref="ProportionalValve0EnablePid"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve0EnablePid register.")]
    public partial class TimestampedProportionalValve0EnablePid
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve0EnablePid"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve0EnablePid.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve0EnablePid"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return ProportionalValve0EnablePid.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the duty cycle for proportional valve 0.
    /// </summary>
    [Description("Set the duty cycle for proportional valve 0.")]
    public partial class ProportionalValve0DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve0DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = 96;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve0DutyCycle"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve0DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve0DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve0DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve0DutyCycle"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve0DutyCycle"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve0DutyCycle"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve0DutyCycle"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve0DutyCycle register.
    /// </summary>
    /// <seealso cref="ProportionalValve0DutyCycle"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve0DutyCycle register.")]
    public partial class TimestampedProportionalValve0DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve0DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve0DutyCycle.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve0DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return ProportionalValve0DutyCycle.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the target flow rate for proportional valve 0.
    /// </summary>
    [Description("Set the target flow rate for proportional valve 0.")]
    public partial class ProportionalValve0TargetFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve0TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 97;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve0TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve0TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve0TargetFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve0TargetFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve0TargetFlowRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve0TargetFlowRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve0TargetFlowRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve0TargetFlowRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve0TargetFlowRate register.
    /// </summary>
    /// <seealso cref="ProportionalValve0TargetFlowRate"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve0TargetFlowRate register.")]
    public partial class TimestampedProportionalValve0TargetFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve0TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve0TargetFlowRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve0TargetFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return ProportionalValve0TargetFlowRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the ADC channel used for the control of proportional valve 1.
    /// </summary>
    [Description("Set the ADC channel used for the control of proportional valve 1.")]
    public partial class ProportionalValve1Adc
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve1Adc"/> register. This field is constant.
        /// </summary>
        public const int Address = 98;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve1Adc"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve1Adc"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve1Adc"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve1Adc"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve1Adc"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve1Adc"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve1Adc"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve1Adc"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve1Adc register.
    /// </summary>
    /// <seealso cref="ProportionalValve1Adc"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve1Adc register.")]
    public partial class TimestampedProportionalValve1Adc
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve1Adc"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve1Adc.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve1Adc"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return ProportionalValve1Adc.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable (1) or disable (0) PID control for proportional valve 1.
    /// </summary>
    [Description("Enable (1) or disable (0) PID control for proportional valve 1.")]
    public partial class ProportionalValve1EnablePid
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve1EnablePid"/> register. This field is constant.
        /// </summary>
        public const int Address = 99;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve1EnablePid"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve1EnablePid"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve1EnablePid"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve1EnablePid"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve1EnablePid"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve1EnablePid"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve1EnablePid"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve1EnablePid"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve1EnablePid register.
    /// </summary>
    /// <seealso cref="ProportionalValve1EnablePid"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve1EnablePid register.")]
    public partial class TimestampedProportionalValve1EnablePid
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve1EnablePid"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve1EnablePid.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve1EnablePid"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return ProportionalValve1EnablePid.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the duty cycle for proportional valve 1.
    /// </summary>
    [Description("Set the duty cycle for proportional valve 1.")]
    public partial class ProportionalValve1DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve1DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = 100;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve1DutyCycle"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve1DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve1DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve1DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve1DutyCycle"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve1DutyCycle"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve1DutyCycle"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve1DutyCycle"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve1DutyCycle register.
    /// </summary>
    /// <seealso cref="ProportionalValve1DutyCycle"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve1DutyCycle register.")]
    public partial class TimestampedProportionalValve1DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve1DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve1DutyCycle.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve1DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return ProportionalValve1DutyCycle.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the target flow rate for proportional valve 1.
    /// </summary>
    [Description("Set the target flow rate for proportional valve 1.")]
    public partial class ProportionalValve1TargetFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve1TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 101;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve1TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve1TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve1TargetFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve1TargetFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve1TargetFlowRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve1TargetFlowRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve1TargetFlowRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve1TargetFlowRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve1TargetFlowRate register.
    /// </summary>
    /// <seealso cref="ProportionalValve1TargetFlowRate"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve1TargetFlowRate register.")]
    public partial class TimestampedProportionalValve1TargetFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve1TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve1TargetFlowRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve1TargetFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return ProportionalValve1TargetFlowRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the ADC channel used for the control of proportional valve 2.
    /// </summary>
    [Description("Set the ADC channel used for the control of proportional valve 2.")]
    public partial class ProportionalValve2Adc
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve2Adc"/> register. This field is constant.
        /// </summary>
        public const int Address = 102;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve2Adc"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve2Adc"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve2Adc"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve2Adc"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve2Adc"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve2Adc"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve2Adc"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve2Adc"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve2Adc register.
    /// </summary>
    /// <seealso cref="ProportionalValve2Adc"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve2Adc register.")]
    public partial class TimestampedProportionalValve2Adc
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve2Adc"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve2Adc.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve2Adc"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return ProportionalValve2Adc.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable (1) or disable (0) PID control for proportional valve 2.
    /// </summary>
    [Description("Enable (1) or disable (0) PID control for proportional valve 2.")]
    public partial class ProportionalValve2EnablePid
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve2EnablePid"/> register. This field is constant.
        /// </summary>
        public const int Address = 103;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve2EnablePid"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve2EnablePid"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve2EnablePid"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve2EnablePid"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve2EnablePid"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve2EnablePid"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve2EnablePid"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve2EnablePid"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve2EnablePid register.
    /// </summary>
    /// <seealso cref="ProportionalValve2EnablePid"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve2EnablePid register.")]
    public partial class TimestampedProportionalValve2EnablePid
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve2EnablePid"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve2EnablePid.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve2EnablePid"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return ProportionalValve2EnablePid.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the duty cycle for proportional valve 2.
    /// </summary>
    [Description("Set the duty cycle for proportional valve 2.")]
    public partial class ProportionalValve2DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve2DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = 104;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve2DutyCycle"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve2DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve2DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve2DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve2DutyCycle"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve2DutyCycle"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve2DutyCycle"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve2DutyCycle"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve2DutyCycle register.
    /// </summary>
    /// <seealso cref="ProportionalValve2DutyCycle"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve2DutyCycle register.")]
    public partial class TimestampedProportionalValve2DutyCycle
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve2DutyCycle"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve2DutyCycle.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve2DutyCycle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return ProportionalValve2DutyCycle.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the target flow rate for proportional valve 2.
    /// </summary>
    [Description("Set the target flow rate for proportional valve 2.")]
    public partial class ProportionalValve2TargetFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve2TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 105;

        /// <summary>
        /// Represents the payload type of the <see cref="ProportionalValve2TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="ProportionalValve2TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ProportionalValve2TargetFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ProportionalValve2TargetFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ProportionalValve2TargetFlowRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve2TargetFlowRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ProportionalValve2TargetFlowRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ProportionalValve2TargetFlowRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ProportionalValve2TargetFlowRate register.
    /// </summary>
    /// <seealso cref="ProportionalValve2TargetFlowRate"/>
    [Description("Filters and selects timestamped messages from the ProportionalValve2TargetFlowRate register.")]
    public partial class TimestampedProportionalValve2TargetFlowRate
    {
        /// <summary>
        /// Represents the address of the <see cref="ProportionalValve2TargetFlowRate"/> register. This field is constant.
        /// </summary>
        public const int Address = ProportionalValve2TargetFlowRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ProportionalValve2TargetFlowRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return ProportionalValve2TargetFlowRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable to freeze PID updates when the final valve is energized.
    /// </summary>
    [Description("Enable to freeze PID updates when the final valve is energized.")]
    public partial class FreezePidUpdates
    {
        /// <summary>
        /// Represents the address of the <see cref="FreezePidUpdates"/> register. This field is constant.
        /// </summary>
        public const int Address = 106;

        /// <summary>
        /// Represents the payload type of the <see cref="FreezePidUpdates"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="FreezePidUpdates"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="FreezePidUpdates"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="FreezePidUpdates"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="FreezePidUpdates"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FreezePidUpdates"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="FreezePidUpdates"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FreezePidUpdates"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// FreezePidUpdates register.
    /// </summary>
    /// <seealso cref="FreezePidUpdates"/>
    [Description("Filters and selects timestamped messages from the FreezePidUpdates register.")]
    public partial class TimestampedFreezePidUpdates
    {
        /// <summary>
        /// Represents the address of the <see cref="FreezePidUpdates"/> register. This field is constant.
        /// </summary>
        public const int Address = FreezePidUpdates.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="FreezePidUpdates"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return FreezePidUpdates.GetTimestampedPayload(message);
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
    /// <seealso cref="CreateQueuedOdorMaskPayload"/>
    /// <seealso cref="CreateOdorSetupTimeUSPayload"/>
    /// <seealso cref="CreateMinOdorDeliveryTimeUSPayload"/>
    /// <seealso cref="CreateMaxOdorDeliveryTimeUSPayload"/>
    /// <seealso cref="CreateMinimumPokeTimeUSPayload"/>
    /// <seealso cref="CreateOdorDwellTimeUSPayload"/>
    /// <seealso cref="CreateCam0PinStatePayload"/>
    /// <seealso cref="CreateCam0FrameRatePayload"/>
    /// <seealso cref="CreateCam0DutyCyclePayload"/>
    /// <seealso cref="CreateEnableCam0TriggerPayload"/>
    /// <seealso cref="CreateCam1PinStatePayload"/>
    /// <seealso cref="CreateCam1FrameRatePayload"/>
    /// <seealso cref="CreateCam1DutyCyclePayload"/>
    /// <seealso cref="CreateEnableCam1TriggerPayload"/>
    /// <seealso cref="CreateEnableValveLedsPayload"/>
    /// <seealso cref="CreateLatestFlowRatePayload"/>
    /// <seealso cref="CreateLatestRawAdcSamplePayload"/>
    /// <seealso cref="CreateEnableAdcSamplingPayload"/>
    /// <seealso cref="CreateLeakAdcChannelPayload"/>
    /// <seealso cref="CreateLeakThresholdPayload"/>
    /// <seealso cref="CreateLeakStatePayload"/>
    /// <seealso cref="CreateManualFlowMeterPayload"/>
    /// <seealso cref="CreateNominalFlowRatePayload"/>
    /// <seealso cref="CreateFlowRateTolerancePayload"/>
    /// <seealso cref="CreateManualFlowMeterStatePayload"/>
    /// <seealso cref="CreateFlowMeterCalibrationsPayload"/>
    /// <seealso cref="CreatePidUpdateFrequencyPayload"/>
    /// <seealso cref="CreatePidGainsPayload"/>
    /// <seealso cref="CreateProportionalValve0AdcPayload"/>
    /// <seealso cref="CreateProportionalValve0EnablePidPayload"/>
    /// <seealso cref="CreateProportionalValve0DutyCyclePayload"/>
    /// <seealso cref="CreateProportionalValve0TargetFlowRatePayload"/>
    /// <seealso cref="CreateProportionalValve1AdcPayload"/>
    /// <seealso cref="CreateProportionalValve1EnablePidPayload"/>
    /// <seealso cref="CreateProportionalValve1DutyCyclePayload"/>
    /// <seealso cref="CreateProportionalValve1TargetFlowRatePayload"/>
    /// <seealso cref="CreateProportionalValve2AdcPayload"/>
    /// <seealso cref="CreateProportionalValve2EnablePidPayload"/>
    /// <seealso cref="CreateProportionalValve2DutyCyclePayload"/>
    /// <seealso cref="CreateProportionalValve2TargetFlowRatePayload"/>
    /// <seealso cref="CreateFreezePidUpdatesPayload"/>
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
    [XmlInclude(typeof(CreateQueuedOdorMaskPayload))]
    [XmlInclude(typeof(CreateOdorSetupTimeUSPayload))]
    [XmlInclude(typeof(CreateMinOdorDeliveryTimeUSPayload))]
    [XmlInclude(typeof(CreateMaxOdorDeliveryTimeUSPayload))]
    [XmlInclude(typeof(CreateMinimumPokeTimeUSPayload))]
    [XmlInclude(typeof(CreateOdorDwellTimeUSPayload))]
    [XmlInclude(typeof(CreateCam0PinStatePayload))]
    [XmlInclude(typeof(CreateCam0FrameRatePayload))]
    [XmlInclude(typeof(CreateCam0DutyCyclePayload))]
    [XmlInclude(typeof(CreateEnableCam0TriggerPayload))]
    [XmlInclude(typeof(CreateCam1PinStatePayload))]
    [XmlInclude(typeof(CreateCam1FrameRatePayload))]
    [XmlInclude(typeof(CreateCam1DutyCyclePayload))]
    [XmlInclude(typeof(CreateEnableCam1TriggerPayload))]
    [XmlInclude(typeof(CreateEnableValveLedsPayload))]
    [XmlInclude(typeof(CreateLatestFlowRatePayload))]
    [XmlInclude(typeof(CreateLatestRawAdcSamplePayload))]
    [XmlInclude(typeof(CreateEnableAdcSamplingPayload))]
    [XmlInclude(typeof(CreateLeakAdcChannelPayload))]
    [XmlInclude(typeof(CreateLeakThresholdPayload))]
    [XmlInclude(typeof(CreateLeakStatePayload))]
    [XmlInclude(typeof(CreateManualFlowMeterPayload))]
    [XmlInclude(typeof(CreateNominalFlowRatePayload))]
    [XmlInclude(typeof(CreateFlowRateTolerancePayload))]
    [XmlInclude(typeof(CreateManualFlowMeterStatePayload))]
    [XmlInclude(typeof(CreateFlowMeterCalibrationsPayload))]
    [XmlInclude(typeof(CreatePidUpdateFrequencyPayload))]
    [XmlInclude(typeof(CreatePidGainsPayload))]
    [XmlInclude(typeof(CreateProportionalValve0AdcPayload))]
    [XmlInclude(typeof(CreateProportionalValve0EnablePidPayload))]
    [XmlInclude(typeof(CreateProportionalValve0DutyCyclePayload))]
    [XmlInclude(typeof(CreateProportionalValve0TargetFlowRatePayload))]
    [XmlInclude(typeof(CreateProportionalValve1AdcPayload))]
    [XmlInclude(typeof(CreateProportionalValve1EnablePidPayload))]
    [XmlInclude(typeof(CreateProportionalValve1DutyCyclePayload))]
    [XmlInclude(typeof(CreateProportionalValve1TargetFlowRatePayload))]
    [XmlInclude(typeof(CreateProportionalValve2AdcPayload))]
    [XmlInclude(typeof(CreateProportionalValve2EnablePidPayload))]
    [XmlInclude(typeof(CreateProportionalValve2DutyCyclePayload))]
    [XmlInclude(typeof(CreateProportionalValve2TargetFlowRatePayload))]
    [XmlInclude(typeof(CreateFreezePidUpdatesPayload))]
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
    [XmlInclude(typeof(CreateTimestampedQueuedOdorMaskPayload))]
    [XmlInclude(typeof(CreateTimestampedOdorSetupTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedMinOdorDeliveryTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedMaxOdorDeliveryTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedMinimumPokeTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedOdorDwellTimeUSPayload))]
    [XmlInclude(typeof(CreateTimestampedCam0PinStatePayload))]
    [XmlInclude(typeof(CreateTimestampedCam0FrameRatePayload))]
    [XmlInclude(typeof(CreateTimestampedCam0DutyCyclePayload))]
    [XmlInclude(typeof(CreateTimestampedEnableCam0TriggerPayload))]
    [XmlInclude(typeof(CreateTimestampedCam1PinStatePayload))]
    [XmlInclude(typeof(CreateTimestampedCam1FrameRatePayload))]
    [XmlInclude(typeof(CreateTimestampedCam1DutyCyclePayload))]
    [XmlInclude(typeof(CreateTimestampedEnableCam1TriggerPayload))]
    [XmlInclude(typeof(CreateTimestampedEnableValveLedsPayload))]
    [XmlInclude(typeof(CreateTimestampedLatestFlowRatePayload))]
    [XmlInclude(typeof(CreateTimestampedLatestRawAdcSamplePayload))]
    [XmlInclude(typeof(CreateTimestampedEnableAdcSamplingPayload))]
    [XmlInclude(typeof(CreateTimestampedLeakAdcChannelPayload))]
    [XmlInclude(typeof(CreateTimestampedLeakThresholdPayload))]
    [XmlInclude(typeof(CreateTimestampedLeakStatePayload))]
    [XmlInclude(typeof(CreateTimestampedManualFlowMeterPayload))]
    [XmlInclude(typeof(CreateTimestampedNominalFlowRatePayload))]
    [XmlInclude(typeof(CreateTimestampedFlowRateTolerancePayload))]
    [XmlInclude(typeof(CreateTimestampedManualFlowMeterStatePayload))]
    [XmlInclude(typeof(CreateTimestampedFlowMeterCalibrationsPayload))]
    [XmlInclude(typeof(CreateTimestampedPidUpdateFrequencyPayload))]
    [XmlInclude(typeof(CreateTimestampedPidGainsPayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve0AdcPayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve0EnablePidPayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve0DutyCyclePayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve0TargetFlowRatePayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve1AdcPayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve1EnablePidPayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve1DutyCyclePayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve1TargetFlowRatePayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve2AdcPayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve2EnablePidPayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve2DutyCyclePayload))]
    [XmlInclude(typeof(CreateTimestampedProportionalValve2TargetFlowRatePayload))]
    [XmlInclude(typeof(CreateTimestampedFreezePidUpdatesPayload))]
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
        public ushort ValveState { get; set; }

        /// <summary>
        /// Creates a message payload for the ValveState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
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
    /// that set the state (on or off) of any auxiliary GPIO pins specified as outputs.
    /// </summary>
    [DisplayName("AuxGPIOStatePayload")]
    [Description("Creates a message payload that set the state (on or off) of any auxiliary GPIO pins specified as outputs.")]
    public partial class CreateAuxGPIOStatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the state (on or off) of any auxiliary GPIO pins specified as outputs.
        /// </summary>
        [Description("The value that set the state (on or off) of any auxiliary GPIO pins specified as outputs.")]
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
        /// Creates a message that set the state (on or off) of any auxiliary GPIO pins specified as outputs.
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
    /// that set the state (on or off) of any auxiliary GPIO pins specified as outputs.
    /// </summary>
    [DisplayName("TimestampedAuxGPIOStatePayload")]
    [Description("Creates a timestamped message payload that set the state (on or off) of any auxiliary GPIO pins specified as outputs.")]
    public partial class CreateTimestampedAuxGPIOStatePayload : CreateAuxGPIOStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the state (on or off) of any auxiliary GPIO pins specified as outputs.
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
    /// that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
    /// </summary>
    [DisplayName("FSMStatePayload")]
    [Description("Creates a message payload that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.")]
    public partial class CreateFSMStatePayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
        /// </summary>
        [Description("The value that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.")]
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
        /// Creates a message that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
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
    /// that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
    /// </summary>
    [DisplayName("TimestampedFSMStatePayload")]
    [Description("Creates a timestamped message payload that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.")]
    public partial class CreateTimestampedFSMStatePayload : CreateFSMStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) (aka reset) or Disable (0) the poke handling state machine. Note that QueuedOdorIndex must be specified first. Disabling and then enabling a previously-enabled FSM will reset it to its starting state.
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
    /// that queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed.
    /// </summary>
    [DisplayName("QueuedOdorMaskPayload")]
    [Description("Creates a message payload that queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed.")]
    public partial class CreateQueuedOdorMaskPayload
    {
        /// <summary>
        /// Gets or sets the value that queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed.
        /// </summary>
        [Description("The value that queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed.")]
        public ushort QueuedOdorMask { get; set; }

        /// <summary>
        /// Creates a message payload for the QueuedOdorMask register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return QueuedOdorMask;
        }

        /// <summary>
        /// Creates a message that queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the QueuedOdorMask register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.QueuedOdorMask.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed.
    /// </summary>
    [DisplayName("TimestampedQueuedOdorMaskPayload")]
    [Description("Creates a timestamped message payload that queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed.")]
    public partial class CreateTimestampedQueuedOdorMaskPayload : CreateQueuedOdorMaskPayload
    {
        /// <summary>
        /// Creates a timestamped message that queued odors (value: odor valve mask) that will be delivered to the odor port given a register poke. After odors have been dispensed, the register will be set to 0, which indicates that new odors are needed.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the QueuedOdorMask register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.QueuedOdorMask.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time alotted (in microseconds) for the vacuum valve to close.
    /// </summary>
    [DisplayName("OdorSetupTimeUSPayload")]
    [Description("Creates a message payload that time alotted (in microseconds) for the vacuum valve to close.")]
    public partial class CreateOdorSetupTimeUSPayload
    {
        /// <summary>
        /// Gets or sets the value that time alotted (in microseconds) for the vacuum valve to close.
        /// </summary>
        [Description("The value that time alotted (in microseconds) for the vacuum valve to close.")]
        public uint OdorSetupTimeUS { get; set; }

        /// <summary>
        /// Creates a message payload for the OdorSetupTimeUS register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return OdorSetupTimeUS;
        }

        /// <summary>
        /// Creates a message that time alotted (in microseconds) for the vacuum valve to close.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OdorSetupTimeUS register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.OdorSetupTimeUS.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time alotted (in microseconds) for the vacuum valve to close.
    /// </summary>
    [DisplayName("TimestampedOdorSetupTimeUSPayload")]
    [Description("Creates a timestamped message payload that time alotted (in microseconds) for the vacuum valve to close.")]
    public partial class CreateTimestampedOdorSetupTimeUSPayload : CreateOdorSetupTimeUSPayload
    {
        /// <summary>
        /// Creates a timestamped message that time alotted (in microseconds) for the vacuum valve to close.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OdorSetupTimeUS register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.OdorSetupTimeUS.FromPayload(timestamp, messageType, GetPayload());
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
    /// that time (in microseconds) that the odor remains in the delivery state.
    /// </summary>
    [DisplayName("OdorDwellTimeUSPayload")]
    [Description("Creates a message payload that time (in microseconds) that the odor remains in the delivery state.")]
    public partial class CreateOdorDwellTimeUSPayload
    {
        /// <summary>
        /// Gets or sets the value that time (in microseconds) that the odor remains in the delivery state.
        /// </summary>
        [Description("The value that time (in microseconds) that the odor remains in the delivery state.")]
        public uint OdorDwellTimeUS { get; set; }

        /// <summary>
        /// Creates a message payload for the OdorDwellTimeUS register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return OdorDwellTimeUS;
        }

        /// <summary>
        /// Creates a message that time (in microseconds) that the odor remains in the delivery state.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OdorDwellTimeUS register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.OdorDwellTimeUS.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time (in microseconds) that the odor remains in the delivery state.
    /// </summary>
    [DisplayName("TimestampedOdorDwellTimeUSPayload")]
    [Description("Creates a timestamped message payload that time (in microseconds) that the odor remains in the delivery state.")]
    public partial class CreateTimestampedOdorDwellTimeUSPayload : CreateOdorDwellTimeUSPayload
    {
        /// <summary>
        /// Creates a timestamped message that time (in microseconds) that the odor remains in the delivery state.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OdorDwellTimeUS register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.OdorDwellTimeUS.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
    /// </summary>
    [DisplayName("Cam0PinStatePayload")]
    [Description("Creates a message payload that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
    public partial class CreateCam0PinStatePayload
    {
        /// <summary>
        /// Gets or sets the value that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
        /// </summary>
        [Description("The value that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
        public byte Cam0PinState { get; set; }

        /// <summary>
        /// Creates a message payload for the Cam0PinState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return Cam0PinState;
        }

        /// <summary>
        /// Creates a message that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Cam0PinState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam0PinState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
    /// </summary>
    [DisplayName("TimestampedCam0PinStatePayload")]
    [Description("Creates a timestamped message payload that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
    public partial class CreateTimestampedCam0PinStatePayload : CreateCam0PinStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Cam0PinState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam0PinState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the frame rate of the camera trigger/ frequency of the PWM signal.
    /// </summary>
    [DisplayName("Cam0FrameRatePayload")]
    [Description("Creates a message payload that set the frame rate of the camera trigger/ frequency of the PWM signal.")]
    public partial class CreateCam0FrameRatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the frame rate of the camera trigger/ frequency of the PWM signal.
        /// </summary>
        [Description("The value that set the frame rate of the camera trigger/ frequency of the PWM signal.")]
        public uint Cam0FrameRate { get; set; }

        /// <summary>
        /// Creates a message payload for the Cam0FrameRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return Cam0FrameRate;
        }

        /// <summary>
        /// Creates a message that set the frame rate of the camera trigger/ frequency of the PWM signal.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Cam0FrameRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam0FrameRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the frame rate of the camera trigger/ frequency of the PWM signal.
    /// </summary>
    [DisplayName("TimestampedCam0FrameRatePayload")]
    [Description("Creates a timestamped message payload that set the frame rate of the camera trigger/ frequency of the PWM signal.")]
    public partial class CreateTimestampedCam0FrameRatePayload : CreateCam0FrameRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the frame rate of the camera trigger/ frequency of the PWM signal.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Cam0FrameRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam0FrameRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
    /// </summary>
    [DisplayName("Cam0DutyCyclePayload")]
    [Description("Creates a message payload that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
    public partial class CreateCam0DutyCyclePayload
    {
        /// <summary>
        /// Gets or sets the value that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
        /// </summary>
        [Description("The value that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
        public float Cam0DutyCycle { get; set; }

        /// <summary>
        /// Creates a message payload for the Cam0DutyCycle register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Cam0DutyCycle;
        }

        /// <summary>
        /// Creates a message that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Cam0DutyCycle register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam0DutyCycle.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
    /// </summary>
    [DisplayName("TimestampedCam0DutyCyclePayload")]
    [Description("Creates a timestamped message payload that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
    public partial class CreateTimestampedCam0DutyCyclePayload : CreateCam0DutyCyclePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Cam0DutyCycle register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam0DutyCycle.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable (1) and disable (0) camera triggering/ the PWM signal.
    /// </summary>
    [DisplayName("EnableCam0TriggerPayload")]
    [Description("Creates a message payload that enable (1) and disable (0) camera triggering/ the PWM signal.")]
    public partial class CreateEnableCam0TriggerPayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) and disable (0) camera triggering/ the PWM signal.
        /// </summary>
        [Description("The value that enable (1) and disable (0) camera triggering/ the PWM signal.")]
        public byte EnableCam0Trigger { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableCam0Trigger register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return EnableCam0Trigger;
        }

        /// <summary>
        /// Creates a message that enable (1) and disable (0) camera triggering/ the PWM signal.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableCam0Trigger register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableCam0Trigger.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable (1) and disable (0) camera triggering/ the PWM signal.
    /// </summary>
    [DisplayName("TimestampedEnableCam0TriggerPayload")]
    [Description("Creates a timestamped message payload that enable (1) and disable (0) camera triggering/ the PWM signal.")]
    public partial class CreateTimestampedEnableCam0TriggerPayload : CreateEnableCam0TriggerPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) and disable (0) camera triggering/ the PWM signal.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableCam0Trigger register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableCam0Trigger.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
    /// </summary>
    [DisplayName("Cam1PinStatePayload")]
    [Description("Creates a message payload that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
    public partial class CreateCam1PinStatePayload
    {
        /// <summary>
        /// Gets or sets the value that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
        /// </summary>
        [Description("The value that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
        public byte Cam1PinState { get; set; }

        /// <summary>
        /// Creates a message payload for the Cam1PinState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return Cam1PinState;
        }

        /// <summary>
        /// Creates a message that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Cam1PinState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam1PinState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
    /// </summary>
    [DisplayName("TimestampedCam1PinStatePayload")]
    [Description("Creates a timestamped message payload that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.")]
    public partial class CreateTimestampedCam1PinStatePayload : CreateCam1PinStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that event is initiated when a rising edge of the camera triggered signal (PWM) is detected. The actual value of the pin doesn't change.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Cam1PinState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam1PinState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the frame rate of the camera trigger/ frequency of the PWM signal.
    /// </summary>
    [DisplayName("Cam1FrameRatePayload")]
    [Description("Creates a message payload that set the frame rate of the camera trigger/ frequency of the PWM signal.")]
    public partial class CreateCam1FrameRatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the frame rate of the camera trigger/ frequency of the PWM signal.
        /// </summary>
        [Description("The value that set the frame rate of the camera trigger/ frequency of the PWM signal.")]
        public uint Cam1FrameRate { get; set; }

        /// <summary>
        /// Creates a message payload for the Cam1FrameRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return Cam1FrameRate;
        }

        /// <summary>
        /// Creates a message that set the frame rate of the camera trigger/ frequency of the PWM signal.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Cam1FrameRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam1FrameRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the frame rate of the camera trigger/ frequency of the PWM signal.
    /// </summary>
    [DisplayName("TimestampedCam1FrameRatePayload")]
    [Description("Creates a timestamped message payload that set the frame rate of the camera trigger/ frequency of the PWM signal.")]
    public partial class CreateTimestampedCam1FrameRatePayload : CreateCam1FrameRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the frame rate of the camera trigger/ frequency of the PWM signal.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Cam1FrameRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam1FrameRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
    /// </summary>
    [DisplayName("Cam1DutyCyclePayload")]
    [Description("Creates a message payload that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
    public partial class CreateCam1DutyCyclePayload
    {
        /// <summary>
        /// Gets or sets the value that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
        /// </summary>
        [Description("The value that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
        public float Cam1DutyCycle { get; set; }

        /// <summary>
        /// Creates a message payload for the Cam1DutyCycle register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Cam1DutyCycle;
        }

        /// <summary>
        /// Creates a message that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Cam1DutyCycle register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam1DutyCycle.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
    /// </summary>
    [DisplayName("TimestampedCam1DutyCyclePayload")]
    [Description("Creates a timestamped message payload that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.")]
    public partial class CreateTimestampedCam1DutyCyclePayload : CreateCam1DutyCyclePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the duty cycle of the PWM. Default and recommend is 0.5 for producing a square wave.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Cam1DutyCycle register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.Cam1DutyCycle.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable (1) and disable (0) camera triggering/ the PWM signal.
    /// </summary>
    [DisplayName("EnableCam1TriggerPayload")]
    [Description("Creates a message payload that enable (1) and disable (0) camera triggering/ the PWM signal.")]
    public partial class CreateEnableCam1TriggerPayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) and disable (0) camera triggering/ the PWM signal.
        /// </summary>
        [Description("The value that enable (1) and disable (0) camera triggering/ the PWM signal.")]
        public byte EnableCam1Trigger { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableCam1Trigger register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return EnableCam1Trigger;
        }

        /// <summary>
        /// Creates a message that enable (1) and disable (0) camera triggering/ the PWM signal.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableCam1Trigger register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableCam1Trigger.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable (1) and disable (0) camera triggering/ the PWM signal.
    /// </summary>
    [DisplayName("TimestampedEnableCam1TriggerPayload")]
    [Description("Creates a timestamped message payload that enable (1) and disable (0) camera triggering/ the PWM signal.")]
    public partial class CreateTimestampedEnableCam1TriggerPayload : CreateEnableCam1TriggerPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) and disable (0) camera triggering/ the PWM signal.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableCam1Trigger register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableCam1Trigger.FromPayload(timestamp, messageType, GetPayload());
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
    /// Represents an operator that creates a message payload
    /// that latest flow rate measurement sample from ADC0-8.
    /// </summary>
    [DisplayName("LatestFlowRatePayload")]
    [Description("Creates a message payload that latest flow rate measurement sample from ADC0-8.")]
    public partial class CreateLatestFlowRatePayload
    {
        /// <summary>
        /// Gets or sets a value that aDC0.
        /// </summary>
        [Description("ADC0")]
        public float ADC0 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC1.
        /// </summary>
        [Description("ADC1")]
        public float ADC1 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC2.
        /// </summary>
        [Description("ADC2")]
        public float ADC2 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC3.
        /// </summary>
        [Description("ADC3")]
        public float ADC3 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC4.
        /// </summary>
        [Description("ADC4")]
        public float ADC4 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC5.
        /// </summary>
        [Description("ADC5")]
        public float ADC5 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC6.
        /// </summary>
        [Description("ADC6")]
        public float ADC6 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC7.
        /// </summary>
        [Description("ADC7")]
        public float ADC7 { get; set; }

        /// <summary>
        /// Creates a message payload for the LatestFlowRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LatestFlowRatePayload GetPayload()
        {
            LatestFlowRatePayload value;
            value.ADC0 = ADC0;
            value.ADC1 = ADC1;
            value.ADC2 = ADC2;
            value.ADC3 = ADC3;
            value.ADC4 = ADC4;
            value.ADC5 = ADC5;
            value.ADC6 = ADC6;
            value.ADC7 = ADC7;
            return value;
        }

        /// <summary>
        /// Creates a message that latest flow rate measurement sample from ADC0-8.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LatestFlowRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LatestFlowRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that latest flow rate measurement sample from ADC0-8.
    /// </summary>
    [DisplayName("TimestampedLatestFlowRatePayload")]
    [Description("Creates a timestamped message payload that latest flow rate measurement sample from ADC0-8.")]
    public partial class CreateTimestampedLatestFlowRatePayload : CreateLatestFlowRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that latest flow rate measurement sample from ADC0-8.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LatestFlowRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LatestFlowRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that latest raw bit measurement sample from ADC0-8.
    /// </summary>
    [DisplayName("LatestRawAdcSamplePayload")]
    [Description("Creates a message payload that latest raw bit measurement sample from ADC0-8.")]
    public partial class CreateLatestRawAdcSamplePayload
    {
        /// <summary>
        /// Gets or sets a value that aDC0.
        /// </summary>
        [Description("ADC0")]
        public float ADC0 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC1.
        /// </summary>
        [Description("ADC1")]
        public float ADC1 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC2.
        /// </summary>
        [Description("ADC2")]
        public float ADC2 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC3.
        /// </summary>
        [Description("ADC3")]
        public float ADC3 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC4.
        /// </summary>
        [Description("ADC4")]
        public float ADC4 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC5.
        /// </summary>
        [Description("ADC5")]
        public float ADC5 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC6.
        /// </summary>
        [Description("ADC6")]
        public float ADC6 { get; set; }

        /// <summary>
        /// Gets or sets a value that aDC7.
        /// </summary>
        [Description("ADC7")]
        public float ADC7 { get; set; }

        /// <summary>
        /// Creates a message payload for the LatestRawAdcSample register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LatestRawAdcSamplePayload GetPayload()
        {
            LatestRawAdcSamplePayload value;
            value.ADC0 = ADC0;
            value.ADC1 = ADC1;
            value.ADC2 = ADC2;
            value.ADC3 = ADC3;
            value.ADC4 = ADC4;
            value.ADC5 = ADC5;
            value.ADC6 = ADC6;
            value.ADC7 = ADC7;
            return value;
        }

        /// <summary>
        /// Creates a message that latest raw bit measurement sample from ADC0-8.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LatestRawAdcSample register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LatestRawAdcSample.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that latest raw bit measurement sample from ADC0-8.
    /// </summary>
    [DisplayName("TimestampedLatestRawAdcSamplePayload")]
    [Description("Creates a timestamped message payload that latest raw bit measurement sample from ADC0-8.")]
    public partial class CreateTimestampedLatestRawAdcSamplePayload : CreateLatestRawAdcSamplePayload
    {
        /// <summary>
        /// Creates a timestamped message that latest raw bit measurement sample from ADC0-8.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LatestRawAdcSample register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LatestRawAdcSample.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable (1) and disable (0) ADC sampling.
    /// </summary>
    [DisplayName("EnableAdcSamplingPayload")]
    [Description("Creates a message payload that enable (1) and disable (0) ADC sampling.")]
    public partial class CreateEnableAdcSamplingPayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) and disable (0) ADC sampling.
        /// </summary>
        [Description("The value that enable (1) and disable (0) ADC sampling.")]
        public byte EnableAdcSampling { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableAdcSampling register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return EnableAdcSampling;
        }

        /// <summary>
        /// Creates a message that enable (1) and disable (0) ADC sampling.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableAdcSampling register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableAdcSampling.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable (1) and disable (0) ADC sampling.
    /// </summary>
    [DisplayName("TimestampedEnableAdcSamplingPayload")]
    [Description("Creates a timestamped message payload that enable (1) and disable (0) ADC sampling.")]
    public partial class CreateTimestampedEnableAdcSamplingPayload : CreateEnableAdcSamplingPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) and disable (0) ADC sampling.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableAdcSampling register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.EnableAdcSampling.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the ADC channel for leak detection. Leak detection is off by Default (-1).
    /// </summary>
    [DisplayName("LeakAdcChannelPayload")]
    [Description("Creates a message payload that set the ADC channel for leak detection. Leak detection is off by Default (-1).")]
    public partial class CreateLeakAdcChannelPayload
    {
        /// <summary>
        /// Gets or sets the value that set the ADC channel for leak detection. Leak detection is off by Default (-1).
        /// </summary>
        [Description("The value that set the ADC channel for leak detection. Leak detection is off by Default (-1).")]
        public sbyte LeakAdcChannel { get; set; }

        /// <summary>
        /// Creates a message payload for the LeakAdcChannel register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public sbyte GetPayload()
        {
            return LeakAdcChannel;
        }

        /// <summary>
        /// Creates a message that set the ADC channel for leak detection. Leak detection is off by Default (-1).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LeakAdcChannel register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LeakAdcChannel.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the ADC channel for leak detection. Leak detection is off by Default (-1).
    /// </summary>
    [DisplayName("TimestampedLeakAdcChannelPayload")]
    [Description("Creates a timestamped message payload that set the ADC channel for leak detection. Leak detection is off by Default (-1).")]
    public partial class CreateTimestampedLeakAdcChannelPayload : CreateLeakAdcChannelPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the ADC channel for leak detection. Leak detection is off by Default (-1).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LeakAdcChannel register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LeakAdcChannel.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the threshold for leak detection in mL/min.
    /// </summary>
    [DisplayName("LeakThresholdPayload")]
    [Description("Creates a message payload that set the threshold for leak detection in mL/min.")]
    public partial class CreateLeakThresholdPayload
    {
        /// <summary>
        /// Gets or sets the value that set the threshold for leak detection in mL/min.
        /// </summary>
        [Description("The value that set the threshold for leak detection in mL/min.")]
        public float LeakThreshold { get; set; }

        /// <summary>
        /// Creates a message payload for the LeakThreshold register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return LeakThreshold;
        }

        /// <summary>
        /// Creates a message that set the threshold for leak detection in mL/min.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LeakThreshold register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LeakThreshold.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the threshold for leak detection in mL/min.
    /// </summary>
    [DisplayName("TimestampedLeakThresholdPayload")]
    [Description("Creates a timestamped message payload that set the threshold for leak detection in mL/min.")]
    public partial class CreateTimestampedLeakThresholdPayload : CreateLeakThresholdPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the threshold for leak detection in mL/min.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LeakThreshold register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LeakThreshold.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the state for leak detection.
    /// </summary>
    [DisplayName("LeakStatePayload")]
    [Description("Creates a message payload that set the state for leak detection.")]
    public partial class CreateLeakStatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the state for leak detection.
        /// </summary>
        [Description("The value that set the state for leak detection.")]
        public byte LeakState { get; set; }

        /// <summary>
        /// Creates a message payload for the LeakState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return LeakState;
        }

        /// <summary>
        /// Creates a message that set the state for leak detection.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LeakState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LeakState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the state for leak detection.
    /// </summary>
    [DisplayName("TimestampedLeakStatePayload")]
    [Description("Creates a timestamped message payload that set the state for leak detection.")]
    public partial class CreateTimestampedLeakStatePayload : CreateLeakStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the state for leak detection.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LeakState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.LeakState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.
    /// </summary>
    [DisplayName("ManualFlowMeterPayload")]
    [Description("Creates a message payload that set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.")]
    public partial class CreateManualFlowMeterPayload
    {
        /// <summary>
        /// Gets or sets the value that set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.
        /// </summary>
        [Description("The value that set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.")]
        public sbyte ManualFlowMeter { get; set; }

        /// <summary>
        /// Creates a message payload for the ManualFlowMeter register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public sbyte GetPayload()
        {
            return ManualFlowMeter;
        }

        /// <summary>
        /// Creates a message that set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ManualFlowMeter register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ManualFlowMeter.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.
    /// </summary>
    [DisplayName("TimestampedManualFlowMeterPayload")]
    [Description("Creates a timestamped message payload that set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.")]
    public partial class CreateTimestampedManualFlowMeterPayload : CreateManualFlowMeterPayload
    {
        /// <summary>
        /// Creates a timestamped message that set which ADC channel (if any) is being used for manual flow meter calibration. Set to -1 for no manual flow meter calibration.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ManualFlowMeter register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ManualFlowMeter.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the nominal flow rate for manual flow meter calibration in mL/min.
    /// </summary>
    [DisplayName("NominalFlowRatePayload")]
    [Description("Creates a message payload that set the nominal flow rate for manual flow meter calibration in mL/min.")]
    public partial class CreateNominalFlowRatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the nominal flow rate for manual flow meter calibration in mL/min.
        /// </summary>
        [Description("The value that set the nominal flow rate for manual flow meter calibration in mL/min.")]
        public float NominalFlowRate { get; set; }

        /// <summary>
        /// Creates a message payload for the NominalFlowRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return NominalFlowRate;
        }

        /// <summary>
        /// Creates a message that set the nominal flow rate for manual flow meter calibration in mL/min.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the NominalFlowRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.NominalFlowRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the nominal flow rate for manual flow meter calibration in mL/min.
    /// </summary>
    [DisplayName("TimestampedNominalFlowRatePayload")]
    [Description("Creates a timestamped message payload that set the nominal flow rate for manual flow meter calibration in mL/min.")]
    public partial class CreateTimestampedNominalFlowRatePayload : CreateNominalFlowRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the nominal flow rate for manual flow meter calibration in mL/min.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the NominalFlowRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.NominalFlowRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the tolerance for flow rate detection (e.g., +-0.1 mL/min).
    /// </summary>
    [DisplayName("FlowRateTolerancePayload")]
    [Description("Creates a message payload that set the tolerance for flow rate detection (e.g., +-0.1 mL/min).")]
    public partial class CreateFlowRateTolerancePayload
    {
        /// <summary>
        /// Gets or sets the value that set the tolerance for flow rate detection (e.g., +-0.1 mL/min).
        /// </summary>
        [Description("The value that set the tolerance for flow rate detection (e.g., +-0.1 mL/min).")]
        public float FlowRateTolerance { get; set; }

        /// <summary>
        /// Creates a message payload for the FlowRateTolerance register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return FlowRateTolerance;
        }

        /// <summary>
        /// Creates a message that set the tolerance for flow rate detection (e.g., +-0.1 mL/min).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the FlowRateTolerance register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FlowRateTolerance.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the tolerance for flow rate detection (e.g., +-0.1 mL/min).
    /// </summary>
    [DisplayName("TimestampedFlowRateTolerancePayload")]
    [Description("Creates a timestamped message payload that set the tolerance for flow rate detection (e.g., +-0.1 mL/min).")]
    public partial class CreateTimestampedFlowRateTolerancePayload : CreateFlowRateTolerancePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the tolerance for flow rate detection (e.g., +-0.1 mL/min).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the FlowRateTolerance register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FlowRateTolerance.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the state for manual flow meter calibration.
    /// </summary>
    [DisplayName("ManualFlowMeterStatePayload")]
    [Description("Creates a message payload that set the state for manual flow meter calibration.")]
    public partial class CreateManualFlowMeterStatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the state for manual flow meter calibration.
        /// </summary>
        [Description("The value that set the state for manual flow meter calibration.")]
        public byte ManualFlowMeterState { get; set; }

        /// <summary>
        /// Creates a message payload for the ManualFlowMeterState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return ManualFlowMeterState;
        }

        /// <summary>
        /// Creates a message that set the state for manual flow meter calibration.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ManualFlowMeterState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ManualFlowMeterState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the state for manual flow meter calibration.
    /// </summary>
    [DisplayName("TimestampedManualFlowMeterStatePayload")]
    [Description("Creates a timestamped message payload that set the state for manual flow meter calibration.")]
    public partial class CreateTimestampedManualFlowMeterStatePayload : CreateManualFlowMeterStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the state for manual flow meter calibration.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ManualFlowMeterState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ManualFlowMeterState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that calibration for the flow meters.
    /// </summary>
    [DisplayName("FlowMeterCalibrationsPayload")]
    [Description("Creates a message payload that calibration for the flow meters.")]
    public partial class CreateFlowMeterCalibrationsPayload
    {
        /// <summary>
        /// Gets or sets a value that a0.
        /// </summary>
        [Description("A0")]
        public float A0 { get; set; }

        /// <summary>
        /// Gets or sets a value that a1.
        /// </summary>
        [Description("A1")]
        public float A1 { get; set; }

        /// <summary>
        /// Gets or sets a value that a2.
        /// </summary>
        [Description("A2")]
        public float A2 { get; set; }

        /// <summary>
        /// Gets or sets a value that a3.
        /// </summary>
        [Description("A3")]
        public float A3 { get; set; }

        /// <summary>
        /// Gets or sets a value that a4.
        /// </summary>
        [Description("A4")]
        public float A4 { get; set; }

        /// <summary>
        /// Gets or sets a value that a5.
        /// </summary>
        [Description("A5")]
        public float A5 { get; set; }

        /// <summary>
        /// Creates a message payload for the FlowMeterCalibrations register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public FlowMeterCalibrationsPayload GetPayload()
        {
            FlowMeterCalibrationsPayload value;
            value.A0 = A0;
            value.A1 = A1;
            value.A2 = A2;
            value.A3 = A3;
            value.A4 = A4;
            value.A5 = A5;
            return value;
        }

        /// <summary>
        /// Creates a message that calibration for the flow meters.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the FlowMeterCalibrations register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FlowMeterCalibrations.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that calibration for the flow meters.
    /// </summary>
    [DisplayName("TimestampedFlowMeterCalibrationsPayload")]
    [Description("Creates a timestamped message payload that calibration for the flow meters.")]
    public partial class CreateTimestampedFlowMeterCalibrationsPayload : CreateFlowMeterCalibrationsPayload
    {
        /// <summary>
        /// Creates a timestamped message that calibration for the flow meters.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the FlowMeterCalibrations register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FlowMeterCalibrations.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the update frequency for the PID controller.
    /// </summary>
    [DisplayName("PidUpdateFrequencyPayload")]
    [Description("Creates a message payload that set the update frequency for the PID controller.")]
    public partial class CreatePidUpdateFrequencyPayload
    {
        /// <summary>
        /// Gets or sets the value that set the update frequency for the PID controller.
        /// </summary>
        [Description("The value that set the update frequency for the PID controller.")]
        public float PidUpdateFrequency { get; set; }

        /// <summary>
        /// Creates a message payload for the PidUpdateFrequency register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return PidUpdateFrequency;
        }

        /// <summary>
        /// Creates a message that set the update frequency for the PID controller.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PidUpdateFrequency register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PidUpdateFrequency.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the update frequency for the PID controller.
    /// </summary>
    [DisplayName("TimestampedPidUpdateFrequencyPayload")]
    [Description("Creates a timestamped message payload that set the update frequency for the PID controller.")]
    public partial class CreateTimestampedPidUpdateFrequencyPayload : CreatePidUpdateFrequencyPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the update frequency for the PID controller.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PidUpdateFrequency register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PidUpdateFrequency.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the PID Kp, Ki, and Kd gains for the controller.
    /// </summary>
    [DisplayName("PidGainsPayload")]
    [Description("Creates a message payload that set the PID Kp, Ki, and Kd gains for the controller.")]
    public partial class CreatePidGainsPayload
    {
        /// <summary>
        /// Gets or sets a value that proportional gain. Default: 2.0.
        /// </summary>
        [Description("Proportional gain. Default: 2.0")]
        public float Kp { get; set; }

        /// <summary>
        /// Gets or sets a value that integral gain. Default: 0.75.
        /// </summary>
        [Description("Integral gain. Default: 0.75")]
        public float Ki { get; set; }

        /// <summary>
        /// Gets or sets a value that derivative gain. Default: 0.18.
        /// </summary>
        [Description("Derivative gain. Default: 0.18")]
        public float Kd { get; set; }

        /// <summary>
        /// Creates a message payload for the PidGains register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public PidGainsPayload GetPayload()
        {
            PidGainsPayload value;
            value.Kp = Kp;
            value.Ki = Ki;
            value.Kd = Kd;
            return value;
        }

        /// <summary>
        /// Creates a message that set the PID Kp, Ki, and Kd gains for the controller.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PidGains register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PidGains.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the PID Kp, Ki, and Kd gains for the controller.
    /// </summary>
    [DisplayName("TimestampedPidGainsPayload")]
    [Description("Creates a timestamped message payload that set the PID Kp, Ki, and Kd gains for the controller.")]
    public partial class CreateTimestampedPidGainsPayload : CreatePidGainsPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the PID Kp, Ki, and Kd gains for the controller.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PidGains register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.PidGains.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the ADC channel used for the control of proportional valve 0.
    /// </summary>
    [DisplayName("ProportionalValve0AdcPayload")]
    [Description("Creates a message payload that set the ADC channel used for the control of proportional valve 0.")]
    public partial class CreateProportionalValve0AdcPayload
    {
        /// <summary>
        /// Gets or sets the value that set the ADC channel used for the control of proportional valve 0.
        /// </summary>
        [Description("The value that set the ADC channel used for the control of proportional valve 0.")]
        public byte ProportionalValve0Adc { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve0Adc register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return ProportionalValve0Adc;
        }

        /// <summary>
        /// Creates a message that set the ADC channel used for the control of proportional valve 0.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve0Adc register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve0Adc.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the ADC channel used for the control of proportional valve 0.
    /// </summary>
    [DisplayName("TimestampedProportionalValve0AdcPayload")]
    [Description("Creates a timestamped message payload that set the ADC channel used for the control of proportional valve 0.")]
    public partial class CreateTimestampedProportionalValve0AdcPayload : CreateProportionalValve0AdcPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the ADC channel used for the control of proportional valve 0.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve0Adc register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve0Adc.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable (1) or disable (0) PID control for proportional valve 0.
    /// </summary>
    [DisplayName("ProportionalValve0EnablePidPayload")]
    [Description("Creates a message payload that enable (1) or disable (0) PID control for proportional valve 0.")]
    public partial class CreateProportionalValve0EnablePidPayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) or disable (0) PID control for proportional valve 0.
        /// </summary>
        [Description("The value that enable (1) or disable (0) PID control for proportional valve 0.")]
        public byte ProportionalValve0EnablePid { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve0EnablePid register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return ProportionalValve0EnablePid;
        }

        /// <summary>
        /// Creates a message that enable (1) or disable (0) PID control for proportional valve 0.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve0EnablePid register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve0EnablePid.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable (1) or disable (0) PID control for proportional valve 0.
    /// </summary>
    [DisplayName("TimestampedProportionalValve0EnablePidPayload")]
    [Description("Creates a timestamped message payload that enable (1) or disable (0) PID control for proportional valve 0.")]
    public partial class CreateTimestampedProportionalValve0EnablePidPayload : CreateProportionalValve0EnablePidPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) or disable (0) PID control for proportional valve 0.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve0EnablePid register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve0EnablePid.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the duty cycle for proportional valve 0.
    /// </summary>
    [DisplayName("ProportionalValve0DutyCyclePayload")]
    [Description("Creates a message payload that set the duty cycle for proportional valve 0.")]
    public partial class CreateProportionalValve0DutyCyclePayload
    {
        /// <summary>
        /// Gets or sets the value that set the duty cycle for proportional valve 0.
        /// </summary>
        [Description("The value that set the duty cycle for proportional valve 0.")]
        public float ProportionalValve0DutyCycle { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve0DutyCycle register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return ProportionalValve0DutyCycle;
        }

        /// <summary>
        /// Creates a message that set the duty cycle for proportional valve 0.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve0DutyCycle register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve0DutyCycle.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the duty cycle for proportional valve 0.
    /// </summary>
    [DisplayName("TimestampedProportionalValve0DutyCyclePayload")]
    [Description("Creates a timestamped message payload that set the duty cycle for proportional valve 0.")]
    public partial class CreateTimestampedProportionalValve0DutyCyclePayload : CreateProportionalValve0DutyCyclePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the duty cycle for proportional valve 0.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve0DutyCycle register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve0DutyCycle.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the target flow rate for proportional valve 0.
    /// </summary>
    [DisplayName("ProportionalValve0TargetFlowRatePayload")]
    [Description("Creates a message payload that set the target flow rate for proportional valve 0.")]
    public partial class CreateProportionalValve0TargetFlowRatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the target flow rate for proportional valve 0.
        /// </summary>
        [Description("The value that set the target flow rate for proportional valve 0.")]
        public float ProportionalValve0TargetFlowRate { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve0TargetFlowRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return ProportionalValve0TargetFlowRate;
        }

        /// <summary>
        /// Creates a message that set the target flow rate for proportional valve 0.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve0TargetFlowRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve0TargetFlowRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the target flow rate for proportional valve 0.
    /// </summary>
    [DisplayName("TimestampedProportionalValve0TargetFlowRatePayload")]
    [Description("Creates a timestamped message payload that set the target flow rate for proportional valve 0.")]
    public partial class CreateTimestampedProportionalValve0TargetFlowRatePayload : CreateProportionalValve0TargetFlowRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the target flow rate for proportional valve 0.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve0TargetFlowRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve0TargetFlowRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the ADC channel used for the control of proportional valve 1.
    /// </summary>
    [DisplayName("ProportionalValve1AdcPayload")]
    [Description("Creates a message payload that set the ADC channel used for the control of proportional valve 1.")]
    public partial class CreateProportionalValve1AdcPayload
    {
        /// <summary>
        /// Gets or sets the value that set the ADC channel used for the control of proportional valve 1.
        /// </summary>
        [Description("The value that set the ADC channel used for the control of proportional valve 1.")]
        public byte ProportionalValve1Adc { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve1Adc register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return ProportionalValve1Adc;
        }

        /// <summary>
        /// Creates a message that set the ADC channel used for the control of proportional valve 1.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve1Adc register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve1Adc.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the ADC channel used for the control of proportional valve 1.
    /// </summary>
    [DisplayName("TimestampedProportionalValve1AdcPayload")]
    [Description("Creates a timestamped message payload that set the ADC channel used for the control of proportional valve 1.")]
    public partial class CreateTimestampedProportionalValve1AdcPayload : CreateProportionalValve1AdcPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the ADC channel used for the control of proportional valve 1.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve1Adc register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve1Adc.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable (1) or disable (0) PID control for proportional valve 1.
    /// </summary>
    [DisplayName("ProportionalValve1EnablePidPayload")]
    [Description("Creates a message payload that enable (1) or disable (0) PID control for proportional valve 1.")]
    public partial class CreateProportionalValve1EnablePidPayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) or disable (0) PID control for proportional valve 1.
        /// </summary>
        [Description("The value that enable (1) or disable (0) PID control for proportional valve 1.")]
        public byte ProportionalValve1EnablePid { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve1EnablePid register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return ProportionalValve1EnablePid;
        }

        /// <summary>
        /// Creates a message that enable (1) or disable (0) PID control for proportional valve 1.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve1EnablePid register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve1EnablePid.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable (1) or disable (0) PID control for proportional valve 1.
    /// </summary>
    [DisplayName("TimestampedProportionalValve1EnablePidPayload")]
    [Description("Creates a timestamped message payload that enable (1) or disable (0) PID control for proportional valve 1.")]
    public partial class CreateTimestampedProportionalValve1EnablePidPayload : CreateProportionalValve1EnablePidPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) or disable (0) PID control for proportional valve 1.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve1EnablePid register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve1EnablePid.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the duty cycle for proportional valve 1.
    /// </summary>
    [DisplayName("ProportionalValve1DutyCyclePayload")]
    [Description("Creates a message payload that set the duty cycle for proportional valve 1.")]
    public partial class CreateProportionalValve1DutyCyclePayload
    {
        /// <summary>
        /// Gets or sets the value that set the duty cycle for proportional valve 1.
        /// </summary>
        [Description("The value that set the duty cycle for proportional valve 1.")]
        public float ProportionalValve1DutyCycle { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve1DutyCycle register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return ProportionalValve1DutyCycle;
        }

        /// <summary>
        /// Creates a message that set the duty cycle for proportional valve 1.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve1DutyCycle register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve1DutyCycle.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the duty cycle for proportional valve 1.
    /// </summary>
    [DisplayName("TimestampedProportionalValve1DutyCyclePayload")]
    [Description("Creates a timestamped message payload that set the duty cycle for proportional valve 1.")]
    public partial class CreateTimestampedProportionalValve1DutyCyclePayload : CreateProportionalValve1DutyCyclePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the duty cycle for proportional valve 1.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve1DutyCycle register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve1DutyCycle.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the target flow rate for proportional valve 1.
    /// </summary>
    [DisplayName("ProportionalValve1TargetFlowRatePayload")]
    [Description("Creates a message payload that set the target flow rate for proportional valve 1.")]
    public partial class CreateProportionalValve1TargetFlowRatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the target flow rate for proportional valve 1.
        /// </summary>
        [Description("The value that set the target flow rate for proportional valve 1.")]
        public float ProportionalValve1TargetFlowRate { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve1TargetFlowRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return ProportionalValve1TargetFlowRate;
        }

        /// <summary>
        /// Creates a message that set the target flow rate for proportional valve 1.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve1TargetFlowRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve1TargetFlowRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the target flow rate for proportional valve 1.
    /// </summary>
    [DisplayName("TimestampedProportionalValve1TargetFlowRatePayload")]
    [Description("Creates a timestamped message payload that set the target flow rate for proportional valve 1.")]
    public partial class CreateTimestampedProportionalValve1TargetFlowRatePayload : CreateProportionalValve1TargetFlowRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the target flow rate for proportional valve 1.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve1TargetFlowRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve1TargetFlowRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the ADC channel used for the control of proportional valve 2.
    /// </summary>
    [DisplayName("ProportionalValve2AdcPayload")]
    [Description("Creates a message payload that set the ADC channel used for the control of proportional valve 2.")]
    public partial class CreateProportionalValve2AdcPayload
    {
        /// <summary>
        /// Gets or sets the value that set the ADC channel used for the control of proportional valve 2.
        /// </summary>
        [Description("The value that set the ADC channel used for the control of proportional valve 2.")]
        public byte ProportionalValve2Adc { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve2Adc register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return ProportionalValve2Adc;
        }

        /// <summary>
        /// Creates a message that set the ADC channel used for the control of proportional valve 2.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve2Adc register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve2Adc.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the ADC channel used for the control of proportional valve 2.
    /// </summary>
    [DisplayName("TimestampedProportionalValve2AdcPayload")]
    [Description("Creates a timestamped message payload that set the ADC channel used for the control of proportional valve 2.")]
    public partial class CreateTimestampedProportionalValve2AdcPayload : CreateProportionalValve2AdcPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the ADC channel used for the control of proportional valve 2.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve2Adc register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve2Adc.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable (1) or disable (0) PID control for proportional valve 2.
    /// </summary>
    [DisplayName("ProportionalValve2EnablePidPayload")]
    [Description("Creates a message payload that enable (1) or disable (0) PID control for proportional valve 2.")]
    public partial class CreateProportionalValve2EnablePidPayload
    {
        /// <summary>
        /// Gets or sets the value that enable (1) or disable (0) PID control for proportional valve 2.
        /// </summary>
        [Description("The value that enable (1) or disable (0) PID control for proportional valve 2.")]
        public byte ProportionalValve2EnablePid { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve2EnablePid register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return ProportionalValve2EnablePid;
        }

        /// <summary>
        /// Creates a message that enable (1) or disable (0) PID control for proportional valve 2.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve2EnablePid register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve2EnablePid.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable (1) or disable (0) PID control for proportional valve 2.
    /// </summary>
    [DisplayName("TimestampedProportionalValve2EnablePidPayload")]
    [Description("Creates a timestamped message payload that enable (1) or disable (0) PID control for proportional valve 2.")]
    public partial class CreateTimestampedProportionalValve2EnablePidPayload : CreateProportionalValve2EnablePidPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable (1) or disable (0) PID control for proportional valve 2.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve2EnablePid register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve2EnablePid.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the duty cycle for proportional valve 2.
    /// </summary>
    [DisplayName("ProportionalValve2DutyCyclePayload")]
    [Description("Creates a message payload that set the duty cycle for proportional valve 2.")]
    public partial class CreateProportionalValve2DutyCyclePayload
    {
        /// <summary>
        /// Gets or sets the value that set the duty cycle for proportional valve 2.
        /// </summary>
        [Description("The value that set the duty cycle for proportional valve 2.")]
        public float ProportionalValve2DutyCycle { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve2DutyCycle register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return ProportionalValve2DutyCycle;
        }

        /// <summary>
        /// Creates a message that set the duty cycle for proportional valve 2.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve2DutyCycle register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve2DutyCycle.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the duty cycle for proportional valve 2.
    /// </summary>
    [DisplayName("TimestampedProportionalValve2DutyCyclePayload")]
    [Description("Creates a timestamped message payload that set the duty cycle for proportional valve 2.")]
    public partial class CreateTimestampedProportionalValve2DutyCyclePayload : CreateProportionalValve2DutyCyclePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the duty cycle for proportional valve 2.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve2DutyCycle register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve2DutyCycle.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the target flow rate for proportional valve 2.
    /// </summary>
    [DisplayName("ProportionalValve2TargetFlowRatePayload")]
    [Description("Creates a message payload that set the target flow rate for proportional valve 2.")]
    public partial class CreateProportionalValve2TargetFlowRatePayload
    {
        /// <summary>
        /// Gets or sets the value that set the target flow rate for proportional valve 2.
        /// </summary>
        [Description("The value that set the target flow rate for proportional valve 2.")]
        public float ProportionalValve2TargetFlowRate { get; set; }

        /// <summary>
        /// Creates a message payload for the ProportionalValve2TargetFlowRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return ProportionalValve2TargetFlowRate;
        }

        /// <summary>
        /// Creates a message that set the target flow rate for proportional valve 2.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ProportionalValve2TargetFlowRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve2TargetFlowRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the target flow rate for proportional valve 2.
    /// </summary>
    [DisplayName("TimestampedProportionalValve2TargetFlowRatePayload")]
    [Description("Creates a timestamped message payload that set the target flow rate for proportional valve 2.")]
    public partial class CreateTimestampedProportionalValve2TargetFlowRatePayload : CreateProportionalValve2TargetFlowRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that set the target flow rate for proportional valve 2.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ProportionalValve2TargetFlowRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.ProportionalValve2TargetFlowRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable to freeze PID updates when the final valve is energized.
    /// </summary>
    [DisplayName("FreezePidUpdatesPayload")]
    [Description("Creates a message payload that enable to freeze PID updates when the final valve is energized.")]
    public partial class CreateFreezePidUpdatesPayload
    {
        /// <summary>
        /// Gets or sets the value that enable to freeze PID updates when the final valve is energized.
        /// </summary>
        [Description("The value that enable to freeze PID updates when the final valve is energized.")]
        public byte FreezePidUpdates { get; set; }

        /// <summary>
        /// Creates a message payload for the FreezePidUpdates register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return FreezePidUpdates;
        }

        /// <summary>
        /// Creates a message that enable to freeze PID updates when the final valve is energized.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the FreezePidUpdates register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FreezePidUpdates.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable to freeze PID updates when the final valve is energized.
    /// </summary>
    [DisplayName("TimestampedFreezePidUpdatesPayload")]
    [Description("Creates a timestamped message payload that enable to freeze PID updates when the final valve is energized.")]
    public partial class CreateTimestampedFreezePidUpdatesPayload : CreateFreezePidUpdatesPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable to freeze PID updates when the final valve is energized.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the FreezePidUpdates register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.DelphiController.FreezePidUpdates.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents the payload of the LatestFlowRate register.
    /// </summary>
    public struct LatestFlowRatePayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LatestFlowRatePayload"/> structure.
        /// </summary>
        /// <param name="aDC0">ADC0</param>
        /// <param name="aDC1">ADC1</param>
        /// <param name="aDC2">ADC2</param>
        /// <param name="aDC3">ADC3</param>
        /// <param name="aDC4">ADC4</param>
        /// <param name="aDC5">ADC5</param>
        /// <param name="aDC6">ADC6</param>
        /// <param name="aDC7">ADC7</param>
        public LatestFlowRatePayload(
            float aDC0,
            float aDC1,
            float aDC2,
            float aDC3,
            float aDC4,
            float aDC5,
            float aDC6,
            float aDC7)
        {
            ADC0 = aDC0;
            ADC1 = aDC1;
            ADC2 = aDC2;
            ADC3 = aDC3;
            ADC4 = aDC4;
            ADC5 = aDC5;
            ADC6 = aDC6;
            ADC7 = aDC7;
        }

        /// <summary>
        /// ADC0
        /// </summary>
        public float ADC0;

        /// <summary>
        /// ADC1
        /// </summary>
        public float ADC1;

        /// <summary>
        /// ADC2
        /// </summary>
        public float ADC2;

        /// <summary>
        /// ADC3
        /// </summary>
        public float ADC3;

        /// <summary>
        /// ADC4
        /// </summary>
        public float ADC4;

        /// <summary>
        /// ADC5
        /// </summary>
        public float ADC5;

        /// <summary>
        /// ADC6
        /// </summary>
        public float ADC6;

        /// <summary>
        /// ADC7
        /// </summary>
        public float ADC7;

        /// <summary>
        /// Returns a <see cref="string"/> that represents the payload of
        /// the LatestFlowRate register.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the payload of the
        /// LatestFlowRate register.
        /// </returns>
        public override string ToString()
        {
            return "LatestFlowRatePayload { " +
                "ADC0 = " + ADC0 + ", " +
                "ADC1 = " + ADC1 + ", " +
                "ADC2 = " + ADC2 + ", " +
                "ADC3 = " + ADC3 + ", " +
                "ADC4 = " + ADC4 + ", " +
                "ADC5 = " + ADC5 + ", " +
                "ADC6 = " + ADC6 + ", " +
                "ADC7 = " + ADC7 + " " +
            "}";
        }
    }

    /// <summary>
    /// Represents the payload of the LatestRawAdcSample register.
    /// </summary>
    public struct LatestRawAdcSamplePayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LatestRawAdcSamplePayload"/> structure.
        /// </summary>
        /// <param name="aDC0">ADC0</param>
        /// <param name="aDC1">ADC1</param>
        /// <param name="aDC2">ADC2</param>
        /// <param name="aDC3">ADC3</param>
        /// <param name="aDC4">ADC4</param>
        /// <param name="aDC5">ADC5</param>
        /// <param name="aDC6">ADC6</param>
        /// <param name="aDC7">ADC7</param>
        public LatestRawAdcSamplePayload(
            float aDC0,
            float aDC1,
            float aDC2,
            float aDC3,
            float aDC4,
            float aDC5,
            float aDC6,
            float aDC7)
        {
            ADC0 = aDC0;
            ADC1 = aDC1;
            ADC2 = aDC2;
            ADC3 = aDC3;
            ADC4 = aDC4;
            ADC5 = aDC5;
            ADC6 = aDC6;
            ADC7 = aDC7;
        }

        /// <summary>
        /// ADC0
        /// </summary>
        public float ADC0;

        /// <summary>
        /// ADC1
        /// </summary>
        public float ADC1;

        /// <summary>
        /// ADC2
        /// </summary>
        public float ADC2;

        /// <summary>
        /// ADC3
        /// </summary>
        public float ADC3;

        /// <summary>
        /// ADC4
        /// </summary>
        public float ADC4;

        /// <summary>
        /// ADC5
        /// </summary>
        public float ADC5;

        /// <summary>
        /// ADC6
        /// </summary>
        public float ADC6;

        /// <summary>
        /// ADC7
        /// </summary>
        public float ADC7;

        /// <summary>
        /// Returns a <see cref="string"/> that represents the payload of
        /// the LatestRawAdcSample register.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the payload of the
        /// LatestRawAdcSample register.
        /// </returns>
        public override string ToString()
        {
            return "LatestRawAdcSamplePayload { " +
                "ADC0 = " + ADC0 + ", " +
                "ADC1 = " + ADC1 + ", " +
                "ADC2 = " + ADC2 + ", " +
                "ADC3 = " + ADC3 + ", " +
                "ADC4 = " + ADC4 + ", " +
                "ADC5 = " + ADC5 + ", " +
                "ADC6 = " + ADC6 + ", " +
                "ADC7 = " + ADC7 + " " +
            "}";
        }
    }

    /// <summary>
    /// Represents the payload of the FlowMeterCalibrations register.
    /// </summary>
    public struct FlowMeterCalibrationsPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowMeterCalibrationsPayload"/> structure.
        /// </summary>
        /// <param name="a0">A0</param>
        /// <param name="a1">A1</param>
        /// <param name="a2">A2</param>
        /// <param name="a3">A3</param>
        /// <param name="a4">A4</param>
        /// <param name="a5">A5</param>
        public FlowMeterCalibrationsPayload(
            float a0,
            float a1,
            float a2,
            float a3,
            float a4,
            float a5)
        {
            A0 = a0;
            A1 = a1;
            A2 = a2;
            A3 = a3;
            A4 = a4;
            A5 = a5;
        }

        /// <summary>
        /// A0
        /// </summary>
        public float A0;

        /// <summary>
        /// A1
        /// </summary>
        public float A1;

        /// <summary>
        /// A2
        /// </summary>
        public float A2;

        /// <summary>
        /// A3
        /// </summary>
        public float A3;

        /// <summary>
        /// A4
        /// </summary>
        public float A4;

        /// <summary>
        /// A5
        /// </summary>
        public float A5;

        /// <summary>
        /// Returns a <see cref="string"/> that represents the payload of
        /// the FlowMeterCalibrations register.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the payload of the
        /// FlowMeterCalibrations register.
        /// </returns>
        public override string ToString()
        {
            return "FlowMeterCalibrationsPayload { " +
                "A0 = " + A0 + ", " +
                "A1 = " + A1 + ", " +
                "A2 = " + A2 + ", " +
                "A3 = " + A3 + ", " +
                "A4 = " + A4 + ", " +
                "A5 = " + A5 + " " +
            "}";
        }
    }

    /// <summary>
    /// Represents the payload of the PidGains register.
    /// </summary>
    public struct PidGainsPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PidGainsPayload"/> structure.
        /// </summary>
        /// <param name="kp">Proportional gain. Default: 2.0</param>
        /// <param name="ki">Integral gain. Default: 0.75</param>
        /// <param name="kd">Derivative gain. Default: 0.18</param>
        public PidGainsPayload(
            float kp,
            float ki,
            float kd)
        {
            Kp = kp;
            Ki = ki;
            Kd = kd;
        }

        /// <summary>
        /// Proportional gain. Default: 2.0
        /// </summary>
        public float Kp;

        /// <summary>
        /// Integral gain. Default: 0.75
        /// </summary>
        public float Ki;

        /// <summary>
        /// Derivative gain. Default: 0.18
        /// </summary>
        public float Kd;

        /// <summary>
        /// Returns a <see cref="string"/> that represents the payload of
        /// the PidGains register.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the payload of the
        /// PidGains register.
        /// </returns>
        public override string ToString()
        {
            return "PidGainsPayload { " +
                "Kp = " + Kp + ", " +
                "Ki = " + Ki + ", " +
                "Kd = " + Kd + " " +
            "}";
        }
    }

    /// <summary>
    /// Valve that can be configured/enabled/disabled
    /// </summary>
    public enum ValveMask : ushort
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
        Valve15 = 15,
        AllValves = 65535
    }

    /// <summary>
    /// Auxiliary GPIO index.
    /// </summary>
    public enum AuxGPIOMask : byte
    {
        AuxGPIO0 = 1,
        AuxGPIO1 = 2,
        AuxGPIO2 = 4,
        AuxGPIO3 = 8,
        AuxGPIO4 = 16,
        AuxGPIO5 = 32,
        AuxGPIO6 = 64,
        AuxGPIO7 = 128
    }
}
