using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Util;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using System.Collections.Generic;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using System.Linq;

namespace IndexerLucen
{
    public class Indexer
    {
        private Directory directory;
        private readonly string PATH_FIELD = "path";
        private readonly string FIELD_NAME_FIELD = "fieldName";

        public Indexer()
        {
            directory = new RAMDirectory();
        }

        public void Build(IEnumerable<(string filePath, string field)> data)
        {
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
            IndexWriter writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

            foreach (var item in data)
            {
                Document doc = new Document();
                doc.Add(new Field(PATH_FIELD, item.filePath, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field(FIELD_NAME_FIELD, item.field, Field.Store.YES, Field.Index.ANALYZED));

                writer.AddDocument(doc);
            }

            writer.Dispose();
        }

        public IEnumerable<(string filePath, string field)> Query(string qText)
        {
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
            QueryParser parser = new QueryParser(Version.LUCENE_30, FIELD_NAME_FIELD, analyzer);
            Query query = parser.Parse(qText);
            IndexSearcher isearcher = new IndexSearcher(directory);
            TopDocs hits = isearcher.Search(query, null, 1000);

            foreach (var item in hits.ScoreDocs)
            {
                var doc = isearcher.Doc(item.Doc);
                var res = (doc.Get(PATH_FIELD), doc.Get(FIELD_NAME_FIELD));
                yield return res;
            }
        }
    }
}
