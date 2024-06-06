# ReminderOnScreen

**ReminderOnScreen** is a Windows application that overlays reminder images on the screen. This application uses OpenCV for image recognition and provides an overlay interface to display text messages associated with the images.

## Features

- Image recognition on the screen using OpenCV.
- Overlay of colored rectangles with text messages.
- Configuration of images and messages through a JSON file.
- Runs in the background without interfering with other applications.

## Prerequisites

- .NET 6.0 SDK
- NuGet Packages:
  - Newtonsoft.Json
  - OpenCvSharp4
  - OpenCvSharp4.Windows

## Installation

1. **Clone the repository**:

    ```bash
    git clone https://github.com/grifone87/Reminderonscreen.git
    cd Reminderonscreen
    ```

2. **Install NuGet packages**:

    Ensure you have the necessary NuGet packages installed. You can do this using the command:

    ```bash
    dotnet restore
    ```

3. **Build the project**:

    To build the project, run:

    ```bash
    dotnet build
    ```

4. **Publish the project**:

    To publish the project in self-contained mode (including the .NET runtime), run:

    ```bash
    dotnet publish -c Release -r win-x64 --self-contained
    ```

## Configuration

1. **Configuration file**:

    The application uses a JSON file to configure the images and messages to be displayed. The `imagesConfig.json` file must be placed in the application directory. Here is an example of how the `imagesConfig.json` file should look:

    ```json
    [
      {
        "Name": "anticEconomica",
        "ImagePath": "images/antieconomica.jpg",
        "Message": "Antieconomica",
        "TextPosition": "below"
      },
      {
        "Name": "anotherImage",
        "ImagePath": "images/anotherimage.jpg",
        "Message": "Another Image",
        "TextPosition": "above"
      }
    ]
    ```

2. **Image paths**:

    The images must be placed in the directory specified in the `ImagePath` field of the JSON file.

## Usage

1. **Run the application**:

    After correctly configuring the JSON file and placing the images in the appropriate folder, run the application:

    ```bash
    dotnet run
    ```

    Or, run the published executable:

    ```bash
    ./bin/Release/net6.0-windows/win-x64/publish/ConsoleApp1.exe
    ```

2. **Interacting with the application**:

    The application will run in the background, overlaying recognized images on the screen with rectangles and text messages.

## Contributions

Contributions are welcome! Feel free to open issues and pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or information, you can contact me on [GitHub](https://github.com/grifone87).
