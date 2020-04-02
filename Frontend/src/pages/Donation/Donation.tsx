import React from "react";
import { Banner } from "../../components/Banner/Banner";
import { Grid } from "@material-ui/core";
import styles from "./Donation.module.scss";
import DonationCard from "../../components/DonationCard/DonationCard";
import { Section } from "../../components/Section/Section";

const DonationPage = () => {
  return (
    <React.Fragment>
      <Banner>
        <Grid container className={styles.banner}>
          <Section>
            <h2>Our Mission</h2>
            <div><p>We help people solve collective action problems through crowdacting</p></div>
          </Section>
        </Grid>
      </Banner>
      <Section>
        <DonationCard />
      </Section>
    </React.Fragment>
  );
};

export default DonationPage;