import React from "react";
import { Button, GhostButton } from "../../components/Button/Button";
import { Facebook, Twitter, LinkedIn, Email } from "../../components/Share";
import { Section } from "../../components/Section";
import Carousel from "../../components/Carousel/Carousel";
import ProjectsList from "../../components/ProjectsList";
import Stats from "../../components/Stats";

const HomePage = () => (
  <React.Fragment>
    <Carousel title="" text="" />
    <Section center color="grey" title="Section title">
      <p dangerouslySetInnerHTML={{ __html: "Section text" }} />
      <GhostButton to="/about">Learn more</GhostButton>
    </Section>
    <Section center title="Time to act">
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
    <div>
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
    </Section>
  </React.Fragment>
);

export default HomePage;