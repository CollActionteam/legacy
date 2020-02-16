import React from "react";
import Layout from "../components/Layout";
import { graphql, StaticQuery } from "gatsby";

import styles from "./index.module.scss";
import { Button, GhostButton } from "../components/Button/Button";
import { Facebook, Twitter, LinkedIn, Email } from "../components/Share";
import { Section } from "../components/Section";
import Carousel from "../components/Carousel/Carousel";
import ProjectsList from "../components/ProjectsList";
import Stats from "../components/Stats";

export default () => (
  <StaticQuery
    query={graphql`
      query HomeQuery {
        site {
          siteMetadata {
            title
          }
        }
        content: allMarkdownRemark(
          filter: { frontmatter: { type: { eq: "home" } } }
        ) {
          edges {
            node {
              html
              frontmatter {
                title
                name
              }
            }
          }
        }
      }
    `}
    render={staticData => {
      const sections = staticData.content.edges.map(e => e.node);
      const carousel = sections.find(s => s.frontmatter.name === "carousel");
      const intro = sections.find(s => s.frontmatter.name === "intro");
      return (
        <Layout>
          <Carousel title={carousel.frontmatter.title} text={carousel.html} />
          <Section center color="grey" title={intro.frontmatter.title}>
            <p dangerouslySetInnerHTML={{ __html: intro.html }} />
            <GhostButton to="/about">Learn more</GhostButton>
          </Section>
          <Section center className={styles.timeToAct} title="Time to act">
            <p>Time to act</p>
          </Section>
          <Section center color="grey" title="Our collective impact">
            <Stats />
          </Section>
          <Section center title="Join a crowdaction">
            <ProjectsList />
            <Button to="/projects/find">Find more projects</Button>
          </Section>
          <Section center color="grey">
            <div className={styles.spread}>
              <div className={styles.spreadBlock}>
                <h2>Spread it further!</h2>
                <ul>
                  <li>
                    <Facebook url="https://www.collaction.org" />
                  </li>
                  <li>
                    <Twitter url="https://www.collaction.org" />
                  </li>
                  <li>
                    <LinkedIn url="https://www.collaction.org" />
                  </li>
                  <li>
                    <Email subject="CollAction" />
                  </li>
                </ul>
              </div>
            </div>
          </Section>
        </Layout>
      );
    }}
  />
);
