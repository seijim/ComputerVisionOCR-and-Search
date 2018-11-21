using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;


namespace WpfAppCvSearch
{
    public class AzureDocumentSearch
    {
        private SearchServiceClient _searchClient;
        private ISearchIndexClient _indexClient;

        public AzureDocumentSearch()
        {
            string searchServiceName = Utils.GetSearchServiceName();
            string apiKey = Utils.GetSearchServiceApiKey();
            string indexName = Utils.GetSearchIndexName();

            // Create an HTTP reference to the catalog index
            _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            _indexClient = _searchClient.Indexes.GetClient(indexName);
        }

        public DocumentSearchResult Search(string searchText, string documentTypeFacet, string projectNameFacet, string projectLocationFacet,
            string postingYearMonthFacet, string tagsFacet, string sortType, int currentPage)
        {
            SearchParameters sp = new SearchParameters()
            {
                SearchMode = SearchMode.Any,
                Top = Utils.GetDefaultPageSize(),
                Skip = currentPage - 1,
                // Limit results
                Select = new List<String>() {"id", "document_type", "project_name", "project_location", "blob_path", "ocr_content",
                        "additional_information", "posting_yearmonth", "posting_date", "post_until", "posting_updated",
                        "process_date", "geo_location", "tags"},
                // Add count
                IncludeTotalResultCount = true,
                // Add search highlights
                HighlightFields = new List<String>() { "ocr_content" },
                HighlightPreTag = "<b>",
                HighlightPostTag = "</b>",
                // Add facets
                Facets = new List<String>() { "document_type", "project_name", "project_location", "posting_yearmonth", "tags" },
            };

            //if (sortType == "featured")
            //{
            //    sp.ScoringProfile = "qcdocScoringFeatured";      // Use a scoring profile
            //    sp.ScoringParameters = new List<ScoringParameter>();
            //    sp.ScoringParameters.Add(new ScoringParameter("featuredParam", new[] { "featured" }));
            //    sp.ScoringParameters.Add(new ScoringParameter("mapCenterParam", GeographyPoint.Create(lon, lat)));
            //}
            //else if (sortType == "salaryDesc")
            //    sp.OrderBy = new List<String>() { "salary_range_from desc" };
            //else if (sortType == "salaryIncr")
            //    sp.OrderBy = new List<String>() { "salary_range_from" };
            //else if (sortType == "mostRecent")
            //    sp.OrderBy = new List<String>() { "posting_date desc" };

            // Add filtering
            string filter = null;
            if (documentTypeFacet != string.Empty)
                filter = "document_type eq '" + documentTypeFacet + "'";
            if (projectNameFacet != "")
            {
                if (filter != null)
                    filter += " and ";
                filter += "project_name eq '" + projectNameFacet + "'";
            }
            if (projectLocationFacet != "")
            {
                if (filter != null)
                    filter += " and ";
                filter += "project_location eq '" + projectLocationFacet + "'";
            }
            if (postingYearMonthFacet != "")
            {
                if (filter != null)
                    filter += " and ";
                filter += "posting_yearmonth eq '" + postingYearMonthFacet + "'";
            }
            if (tagsFacet != "")
            {
                if (filter != null)
                    filter += " and ";
                filter += "tags eq '" + tagsFacet + "'";
            }

            sp.Filter = filter;

            return _indexClient.Documents.Search(searchText, sp);
        }

    }
}
