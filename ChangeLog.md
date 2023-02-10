# V1.5.0
* Added : `Critical` Icon For `MessagePrompt`.
* Added : `BoolToValueConverter` To Convert A Boolean To Any Value.
* Added : `FillerButton` For All Your Filter Needs.
* Added : Fully Functional ResizeGrip - `ResizeGrip`.
* Added : CheckBox Style TreeViewItem - `CheckBoxTreeViewItem`.
* Added : Search Text Box - `SearchBox`.
* Added : Popup Search Component - `FindBox`.
* Added : Some Routed Commands - `ExCom.Exit`, `ExCom.Refrech`.
* Added : New And Improved `DatePicker` Component.
* Added : `TimePicker` Component.
* Added : Auto Select All On Focus For `DigitBox`.
* Added : Arrow Keys Up And Down Now Increases And Decreases `DigitBox` Value.
* Added : IsReadOnly Property To `NumericUpDown`.
* Added : `ChomboBox Styles` For Windows 10 + 11.
* Added : `ListBox And ListView Styles` For Windows 10 + 11.
* Added : Windows 11 `ScrolBar Style`.
* Added : `Excel Styled Datagrid`.
* Added : ImageAlignment Enum For Alignment Of The Image In The `ImageButton` Component. Now With Center Alignment!
###
* Fixed : Windows 11 `ScollBar Style` To Windows 11 `DataGrid Style`.
* Fixed : Try Catch Blocks In `DataGrid Styles` No Longer Breaks On Fail.
* Fixed : Tab Will No Longer Stop On `NumericUpDown` & `CheckBoxGroup` $ `RadioGroupBox` Components And Will Pass Through To Its Content.
* Fixed : `ImageButton` & `CheckBoxGroup` & `RadioGroupBox` Will Accept Enter Key As A Mouse Click.
* Fixed : `DateToStringConverter` Now Has A Higher Chance Of Successfully Converting String Back To DateTime And Will Return `null` On Fail.
###
* Changed : Renamed DoubleConverter To `DoubleToStringConverter`.
* Changed : Renamed DateConverter To `DateToStringConverter`.
* Changed : Renamed `ImageButton` IconAlignment Property To `ImageAlignment`.
* Changed : `ImageAlignment` Property Now Uses Custom Enum + Visual Improvements.
* Changed : `ImageButton` Now Uses RoutedEventArgs Insted Of EventArgs For The Click Event.
* Changed : `ExInputs` Button Panel Background Colour To A Lighter Gray.
* Changed : `ErrorIcon` To Resemble A Error Icon More Accurately.
###
* Removed : Tab Stop For `NumericUpDown` Increase And Decrease Buttons.
* Removed : The Old `ExInputs` Object.
* Removed : PresentationFramework.Aero Assembly.
###
* Added \* : Overhauled The `PropertyView` Component Set.
###
* General : Code Inprovements.

# V1.4.1
* Added : BasicButton Presets |
* Fixed : DataGrid Styles Interfering With Eachother |
* Fixed : Input Prompt Sizing |
* Changed : Updated MessagePromp Icons |

# V1.4.0
* Info : PropertyViewItem Is Still A WIP
* Info : ScrollBarStyles & LisBoxStyles Is WIP
* Added : ClearCurrentConsoleLine | Extras\ExFun
* Added : Roud Function For Double | Extras\ExMath\Round
* Added : New Enum For Groupbox Components | Extras\ExEnums\SelectionType
* Added : CheckBoxGroup. A Missing Component In WPF Framework | ExComponents\CheckBoxGroup
* Added : RadioGroupBox. A Missing Component In WPF Framework | ExComponents\RadioGroupBox
* Added : 3 New Styles For DataGrid Component | ExStyles\
* Changed : NumericUpDown Code Improvements | ExComponents\NumericUpDown\

# V1.3.0
* Info : ExInputs Object Will Not Get Any Magor Updates And Will Be Removed
* Info : PropertyViewItem Will Be Getting An Overhall / Rework
* Fixed : Visual Improvements | ExInputs\ExInputs\
* Fixed : DigitBox Not Allowing Negative Values | ExComponents\DigitBox\
* Added : Descriptions To NumericUpDown Properties | ExComponents\NumericUpDown\
* Added : Descriptions To PropertyViewItem Properties | ExComponents\PropertyViewItem\
* Added : XML Tips For PropertyViewItem | ExComponents\PropertyViewItem\
* Added : ImageButton Control | ExComponents\ImageButton
* Added : IExInput Interface For Adv Custom Inputs | ExInputs\IExInputs
* Added : ExInputBase For Custom Inputs | ExInputs\ExInputBase
* Added : TextPrompt, ComboPrompt, NumberPrompt, DateTimePrompt | ExInputs\
* Added : Custom Click Events To BasicButton | ExInputs\BasicButtons\
* Added : Accept & Cancel Click Events To BasicButton | ExInputs\BasicButtons\
* Added : IndexOfDictionary | Extras\ExFun\IndexOfDictionary
* Added : ToWeek, ToMonth Functions | Extras\ExFun\
* Added : DoubleConverter, DateConverter For WPF Binding Converters | ExConverter\ExDataConverter\
* Changed : Split ExInput Prompts Into Seperate Objects | ExInputs\

# V1.2.0
* Fixed : NumericUpDown When Enter Key Is Press The Value Will Be Entered | ExComponents\NumericUpDown
* Fixed : Setting NumericUpDown Bruchs Will Change The Bruchs | ExComponents\NumericUpDown
* Fixed : When NumericUpDown Value Is Above Or Below Its Max And Min Value It Will Push Back To Its Max Or Min Value | ExComponents\NumericUpDown
* Added : A NoVal Funtion To Skip Validation For CanExecuteFun Parameter | Extrass\EXICom
* Added : A List To ObservableCollection Converter | Extrass\ExFun\ListToObservableCollection
* Added : A PropertyViewItem WPF Component | ExComponents\PropertyViewItem
* Added : A New Enum For PropertyViewItem | Extras\ExEnums\PropertyType
* Changed : Sepperated Component Styles From Generic Into There Own Files | Themes\
* General : Code Inprovements

# V 1.1.1
* Added .Net Framework 4.7.2 Support

# V 1.1.0
* Fixed : LoadFile Function Now Creates The ExistingFile Directory : Extrass\ExFun\LoadFile
* Added : A New Logo For Library
* Added : 2 New Icons For NumericUpDown Buttons | ExComponents\ExComponentsRes\
* Added : A Matod To Copy A Directory | Extras\ExFun\CopyDir
* Added : TextAlignment Property To NumericUpDown | ExComponents\NumericUpDown
* Changed : NumericUpDown Buttons Content To Icons | ExComponents\NumericUpDown
* General : Code Optimizations

# V 1.0.0
* Seperated All Enums Into A Seperate File
* Added : A Customizebel Button | ExInputs\BasicButtons
* Added : 3 New Input Types | ExInputs\ExInputs\
* Added : 5 New Icons For Message Input Prompt | ExInputs\ExInputsRes\
* Added : A Method To Load A File In Your Project To The Client PC | Extrass\ExFun\LoadFile
* Added : A Method To Check Internet Connection | Extrass\ExFun\ConnectionChecker
* Added : A Math Methed To Round Values To A Floating Point | Extrass\ExMath\Round
* Added : A File Log Object | Extrass\ExLog
* Added : A Settings Manager Object | Extrass\ExSettings
* Added : A Console Command Object | Extrass\ExCMD
* Added : A ICommand Object For Easy Custom Commands | Extrass\EXICom
* Added : A DigitBox WPF Component (A TextBox But For Numbers) | ExComponents\
* Added : A NumericUpDown  WPF Component (Adv Model Of A DigitBox) | ExComponents\
* Added : A BindableToolStripMenuItem WinForms Component | ExComponents\
