using ETL.Domain.Config;
using ETL.Domain.MetaDataModels;
using ETL.Domain.Targets.DbTargets;
using Load.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Writers.DatabaseInsertingLogic
{
    public class TableDependencySorter : ITableDependencySorter
    {
        public List<TargetTableConfig> SortByDependency(List<TargetTableConfig> tableConfigs, DatabaseMetaData metadata)
        {
            if (metadata == null)
                throw new InvalidOperationException("Database metadata is required for sorting tables.");

            var targetTableNames = tableConfigs
                .Select(t => t.TargetTable)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var relevantMetadataTables = metadata.Tables
                .Where(t => targetTableNames.Contains(t.TableName))
                .ToList();

            var sortedMetadataTables = TopologicalSort(relevantMetadataTables);

            var sortedTargetTables = sortedMetadataTables
                .Select(metadataTable => tableConfigs
                    .FirstOrDefault(t => t.TargetTable.Equals(metadataTable.TableName, StringComparison.OrdinalIgnoreCase)))
                .Where(t => t != null)
                .ToList();

            return sortedTargetTables;
        }


        private List<TableMetadata> TopologicalSort(List<TableMetadata> tables)
        {
            var sorted = new List<TableMetadata>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void Visit(TableMetadata table)
            {
                if (visited.Contains(table.TableName))
                    return;

                if (table.ForeignKeys != null)
                {
                    foreach (var fk in table.ForeignKeys)
                    {
                        var parent = tables.FirstOrDefault(t => t.TableName.Equals(fk.ReferencedTable, StringComparison.OrdinalIgnoreCase));
                        if (parent != null)
                            Visit(parent);
                    }
                }

                if (!visited.Contains(table.TableName))
                {
                    sorted.Add(table);
                    visited.Add(table.TableName);
                }
            }

            foreach (var table in tables)
                Visit(table);

            return sorted;
        }
    }
}

