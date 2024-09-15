[![Documentation](https://img.shields.io/badge/docs-here-informational)](https://universalis.app/docs)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/f328839ff36f47f7a5672856740d9c00)](https://app.codacy.com/gh/Universalis-FFXIV/Universalis?utm_source=github.com&utm_medium=referral&utm_content=Universalis-FFXIV/Universalis&utm_campaign=Badge_Grade_Settings)
[![Security Headers](https://img.shields.io/security-headers?url=https%3A%2F%2Funiversalis.app)](https://securityheaders.com/?q=https%3A%2F%2Funiversalis.app&followRedirects=on)

# Universalis

trigger CR

A crowdsourced market board aggregator for the game FINAL FANTASY XIV.

## API Reference
Please refer to the [documentation](https://universalis.app/docs) for basic usage information.

## API Development
Developing and testing the API server requires [Visual Studio 2022 Preview](https://docs.microsoft.com/en-us/visualstudio/releases/2022/release-notes-preview), as it targets .NET 6.

This application uses some F# code, which needs to be built before IntelliSense can navigate it. If you get any undefined references to F# code, just build the `Universalis.DataTransformations` project.

## Frontend Development
The frontend is housed on our [mogboard repo](https://github.com/Universalis-FFXIV/mogboard-next), where contributions are welcome.

## Upload Software Development
Please see goat's [ACT plugin](https://github.com/goaaats/universalis_act_plugin) for an example of how to collect and upload market board data.

## Development
Requires .NET 6, PostgreSQL, [MariaDB](https://mariadb.org/download/), and [Redis](https://redis.io/download). A development environment is provided as a Docker Compose specification in the `devenv` folder for simpler setup.

MariaDB commands:
```mysql
CREATE DATABASE `dalamud`;
CREATE USER 'dalamud'@localhost IDENTIFIED BY 'dalamud';
GRANT ALL PRIVILEGES ON `dalamud`.* TO 'dalamud'@localhost IDENTIFIED BY 'dalamud';
FLUSH PRIVILEGES;
```
