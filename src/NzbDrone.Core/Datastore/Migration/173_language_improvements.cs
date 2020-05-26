using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;
using NzbDrone.Core.Languages;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(173)]
    public class language_improvements : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            // Use original language to set default language fallback for releases
            // Set all to English (1) on migration to ensure default behavior persists until refresh
            Alter.Table("Movies").AddColumn("OriginalLanguage").AsInt32().WithDefaultValue((int)Language.English);

            Alter.Table("Movies").AddColumn("Recommendations").AsString().WithDefaultValue("[]");

            Alter.Table("NamingConfig").AddColumn("RenameMovies").AsBoolean().WithDefaultValue(0);

            Execute.Sql("UPDATE NamingConfig SET RenameMovies=RenameEpisodes");

            Delete.Column("RenameEpisodes").FromTable("NamingConfig");

            // Should only throw unique if same movie, same language, same title
            // We don't want to prevent translations from being added
            Execute.Sql("DROP INDEX IF EXISTS \"IX_AlternativeTitles_CleanTitle\"");
        }
    }
}
