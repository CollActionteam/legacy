import React from "react";
import Layout from "../components/Layout";
import { graphql, StaticQuery } from "gatsby";
import { CallToAction } from "../components/CallToAction";
import { Hidden, Grid } from "@material-ui/core";

import styles from "./index.module.scss";
import { CrowdactingSteps } from "../components/CrowdactingSteps";
import { StartProjectSteps } from "../components/StartProjectSteps";
import { Button, GhostButton } from "../components/Button";
import { Facebook, Twitter, LinkedIn, Email } from "../components/Share";
import { Section } from "../components/Section";
import Carousel from "../components/Carousel";
import ProjectsList from "../components/ProjectsList";

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
          <Section color="grey" title={intro.frontmatter.title}>
            <p dangerouslySetInnerHTML={{ __html: intro.html }}></p>
            <GhostButton to="/about">Learn more</GhostButton>
          </Section>
          <Section className={styles.timeToAct} title="Time to act">
            <StartProjectSteps />
          </Section>
          <Section color="grey" title="Our collective impact">
            <p>Stats</p>
          </Section>
          <Section title="Join a crowdaction">
            <ProjectsList />
            <Button to="/projects/find">Find more projects</Button>
          </Section>
          <Section color="grey">
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
