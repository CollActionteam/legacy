import React from "react";
import { Banner } from "../../../components/Banner/Banner";
import { Section } from "../../../components/Section";
import { StartProjectSteps } from "../../../components/StartProjectSteps";
import { Button, SecondaryButton } from "../../../components/Button/Button";
import { Faq } from "../../../components/Faq";
import { Grid } from "@material-ui/core";

import styles from "./Start.module.scss";

const StartProjectPage = () => {
  const intro = "This will be the intro";
  const faqs = [
    {
      name: "Dummy faq",
      html: "Dummy faq html"
    }
  ]
  return (
    <React.Fragment>
      <Banner>
        <div className={styles.banner}></div>
      </Banner>
      <Section className={styles.introBlock}>
        <span dangerouslySetInnerHTML={{ __html: intro }}></span>
        <div className={styles.cta}>
          <SecondaryButton to="/projects/create">
            Start a project
          </SecondaryButton>
        </div>
      </Section>

      <Section color="grey" className={styles.howItWorks} title="How it works">
        <StartProjectSteps />
        <Button to="/projects/create">Start a project</Button>
      </Section>
      <Grid container>
        <Grid item md={2}></Grid>
        <Grid item xs={12} md={8}>
          <Section className={styles.faq} title="Frequently Asked Questions">
            {faqs.map((faq: any) => (
              <Faq
                key={faq.name}
                title={faq.name}
                content={faq.html}
              ></Faq>
            ))}
          </Section>
        </Grid>
      </Grid>
    </React.Fragment>
  );
};

export default StartProjectPage;
