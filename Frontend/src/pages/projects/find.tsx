import React, { useState } from "react";
import Layout from "../../components/Layout";
import { Banner } from "../../components/Banner";
import { graphql, StaticQuery } from "gatsby";
import { useQuery } from "react-apollo";
import ProjectsList from "../../components/ProjectsList";
import gql from "graphql-tag";
import { Section } from "../../components/Section";
import { ProjectStatusFilter } from "../../api/types";

export default () => (
  <StaticQuery
    query={graphql`
      query FindQuery {
        photos: allFindprojectYaml(
          filter: { name: { eq: "findprojectphotos" } }
        ) {
          edges {
            node {
              findprojectphoto
              name
            }
          }
        }
      }
    `}
    render={staticData => {
      const photos = staticData.photos.edges
        .map(e => e.node)
        .find(n => n.name === "findprojectphotos");

      const [category, setCategory] = useState("");
      const [status, setStatus] = useState(ProjectStatusFilter.Active);
      const { data, loading } = useQuery(GET_CATEGORIES);

      const handleCategoryChange = (e: React.ChangeEvent) => {
        setCategory((e.target as any).value.toString());
      };

      const handleStatusChange = (e: React.ChangeEvent) => {
        setStatus((e.target as any).value);
      };

      return (
        <Layout>
          <Banner photo={photos.findprojectphoto} dots={true}>
            <Section>
              {loading ? (
                <div>Loading dropdown...</div>
              ) : (
                <h1>
                  <span>Show me </span>
                  {category}
                  <select onChange={handleCategoryChange}>
                    <option value="">All</option>
                    {data
                      ? data.categories.map((c, i) => (
                          <option key={i} value={c.id}>
                            {c.name}
                          </option>
                        ))
                      : null}
                  </select>

                  <span>projects which are</span>

                  <select onChange={handleStatusChange}>
                    <option value={ProjectStatusFilter.Active}>Open</option>
                    <option value={ProjectStatusFilter.Closed}>Closed</option>
                    <option value={ProjectStatusFilter.ComingSoon}>
                      Coming soon
                    </option>
                  </select>
                </h1>
              )}
            </Section>
          </Banner>
          <Section>
            <ProjectsList categoryId={category} status={status} />
          </Section>
        </Layout>
      );
    }}
  />
);

const GET_CATEGORIES = gql`
  query {
    categories {
      id
      name
    }
  }
`;
