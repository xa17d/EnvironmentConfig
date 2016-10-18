EnvironmentConfig is a simple configuration-file-location concept that also allows multiple developers to work with different application configurations on the same repository. EnvironmentConfig does not aim to replace other configuration files, but provides a concept to easily find a configuration file. You can use EnvironmentConfig to locate the real config file.

## Concept
A file named `environment.config` is searched down the path beginning with the folder of the executing assembly.
**Example:**
The assembly is located here:
`C:/Projects/MySolution/MyProject1/bin/Debug/MyProject1.exe`
You could place `environment.config` in any parent folder like:

* `C:/Projects/MySolution/MyProject1/bin/Debug/environment.config`
* `C:/Projects/MySolution/MyProject1/bin/environment.config`
* `C:/Projects/MySolution/MyProject1/environment.config`	- this is recommended for a Project configuration
* `C:/Projects/MySolution/environment.config`	- this is recommended for a solution wide configuration
* `C:/Projects/environment.config`		- configuration for all projects. Not recommended.
* `C:/environment.config`	- configuration for all projects in `C:/`. Not recommended.

The `environment.config` can be different for every user. So do not add it to the repository, add it to `.gitignore`. Instead create a file named `environment.default.config`. If a parent folder contains no `environment.config` but a `environment.default.config` the `environment.default.config` ist tried to be copied to `environment.config`. If it fails (e.g. due to write permission reasons), the `environment.default.config` is used. Otherwise the settings from the newly copied `environment.config` is used.

## Motivations
The search down the folder structure is used, because most of the times the `/bin`, `/debug` or `/release` folders are not committed. That means configuration files placed there won't be commited to the repository. With EnvironmentConfig you can place the configuration in any parent folder that is commited to the repository. Also different users or IDEs prefer different output folders for the compiler. When the configuration file can be placed in any parent folder, the exact structure does not matter. The assembly always can find the configuration file.

The users or developers may all want to work on the same repository but work with different configurations. Imagine:

* The developer works with `database="localhost/dev"`,
* The tester works with `database="test_server/test"`,
* And at the user it works with `database="company_server/solution1"`,

So the configurations must not be pushed to the repository. That's the reason why `environment.config` should be added to .gitignore. But you should add an `environment.default.config` instead and push it to the repository. So the user knows where the config should be placed and also has the default example configuration.

## Example Usage
* copy `EnvironmentConfig.cs` into your project
* add to `.gitignore`:
```
# EnvironmentConfig
environment.config
```
* in a parent folder, create the file `environment.default.config`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<EnvironmentConfig>
  <images>{environment}/img</images>
</EnvironmentConfig>
```
* also create the folder `img` in the same folder as `environment.default.config`.
* in your code:
```cs
EnvironmentConfig config = EnvironmentConfig.Find();
string images = config.XmlGetPath("images");
```
* Done!
