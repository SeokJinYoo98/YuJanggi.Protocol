# YuJanggi Protocol for Unity

YuJanggiCommon.dll (netstandard2.1) network contracts, packaged for the existing YuJanggi Unity project.

## Runtime dependencies

This package deliberately does not bundle third-party JSON DLLs. The host must provide compatible System.Text.Json (built against NuGet 8.0.5), System.Text.Encodings.Web and their runtime dependencies. Keep the existing YuJanggi Unity JSON runtime plugins and verify a Player build before upgrading. This is not a standalone JSON dependency installer.

Remove the old Assets/Plugins/YuJanggiCommon/YuJanggiCommon.dll and its meta when switching to this package. Do not load both copies. Keep JSON dependency DLLs until their ownership is migrated separately.

Install the immutable Git tag:
https://github.com/SeokJinYoo98/YuJanggi.Protocol.git#upm/v1.0.0

Alternatively download the release .tgz and use Package Manager > Add package from tarball.
Versions above are examples; use an actually published version.
