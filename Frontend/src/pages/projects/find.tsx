import React, { useState } from "react";
import Layout from "../../components/Layout";
import { Container } from "@material-ui/core";
import { Banner } from "../../components/Banner";
import { graphql, StaticQuery } from "gatsby";
import { useQuery } from "react-apollo";
import ProjectsList from "../../components/ProjectsList";
import gql from "graphql-tag";
import Section from "../../components/Section";

export default () => (
  <StaticQuery
    query={graphql`
      query FindQuery {
        photos: allHomeYaml(filter: { name: { eq: "photos" } }) {
          edges {
            node {
              bannerphoto
            }
          }
        }
      }
    `}
    render={staticData => {
      const photos = staticData.photos.edges
        .map(e => e.node)
        .find(n => (n.name = "photos"));

      const [category, setCategory] = useState(null);
      const { data, loading } = useQuery(GET_CATEGORIES);

      const handleCategoryChange = (e: React.ChangeEvent) => {
        setCategory((e.target as any).value.toString());
      };

      return (
        <Layout>
          <Banner photo={photos.bannerphoto}>
            <Container>
              {loading ? (
                <div>Loading dropdown...</div>
              ) : (
                <select onChange={handleCategoryChange}>
                  <option value="">All</option>
                  {data.categories.map((c, i) => (
                    <option key={i} value={c.id}>
                      {c.name}
                    </option>
                  ))}
                </select>
              )}
            </Container>
          </Banner>
          <Section>
            <ProjectsList categoryId={category} />
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
