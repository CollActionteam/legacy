import React from "react";
import {Banner} from "../../components/Banner/Banner";
import {Box, Grid} from "@material-ui/core";
import styles from "./Donation.module.scss";
import DonationCard from "../../components/DonationCard/DonationCard";
import {Section} from "../../components/Section/Section";
import Helmet from "react-helmet";
import DonationFAQ from "./DonationFAQ";

const DonationPage = () => {
    return (
        <React.Fragment>
            <Helmet>
                <title>Donate To CollAction</title>
                <meta name="description" content="Donate To CollAction"/>
            </Helmet>
            <Banner>
                <Grid container className={styles.banner}>
                    <Section>
                        <h2>Our Mission</h2>
                        <h3 className={styles.mission}>We help people solve collective action problems through
                            crowdacting</h3>
                    </Section>
                </Grid>
            </Banner>
            <Section>
                <Grid container spacing={10}>
                    <Grid item sm={12} md={6}>
                        <DonationCard/>
                    </Grid>
                    <Grid item sm={12} md={6}>
                        <DonationFAQ/>
                    </Grid>
                </Grid>
            </Section>
        </React.Fragment>
    );
};

export default DonationPage;