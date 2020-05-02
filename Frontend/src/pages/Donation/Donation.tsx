import React from "react";
import { Banner } from "../../components/Banner/Banner";
import { Grid} from "@material-ui/core";
import styles from "./Donation.module.scss";
import DonationCard from "../../components/DonationCard/DonationCard";
import { Section } from "../../components/Section/Section";
import { Helmet } from "react-helmet";
import { Faq } from "../../components/Faq/Faq";
import { RouteComponentProps } from "react-router-dom";
import {Alert} from "../../components/Alert/Alert";
import DonationThankYouCard from "../../components/DonationCard/DonationThankYouCard";

type TParams = {
    match?: any,
}

const DonationPage = ({ match } : TParams) => {
    const { thankyou } = match && match.params;

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
                        {thankyou ?
                            <DonationThankYouCard/> :
                            <DonationCard/>
                        }
                    </Grid>
                    <Grid item sm={12} md={6}>
                        <h2>Frequently Asked Questions</h2>
                        <br />

                        <Faq title="Why should I donate?" collapsed={true} faqId="why_should_i_donate">
                            <ul>
                                <li>Our goal is to move millions of people to act for good by launching the crowdacting
                                    movement. Whereas back in the day, people turned to politicians and policy makers to fix the
                                    world's social and environmental problems (with different levels of success :) ), we think
                                    it's time for a new approach. With crowdacting, we take matters into our own collective
                                    hands.
                                </li>
                                <li>But in order to reach our ambitious goals, we need you support. CollAction is a non profit
                                    organization. We keep costs super low with the support of our amazing team of volunteers and
                                    great pro bono supporters. However, there are still certain costs that need to be covered
                                    (you can find an overview of our financials here). We don't like to be dependent on
                                    subsidies and we believe it is vital to remain independent from commercial interests. Hence,
                                    we ask for contributions from the crowd to survive, scale our impact, and remain
                                    independent.
                                </li>
                            </ul>
                        </Faq>

                        <Faq title="What will my donation be spent on?" collapsed={true} faqId="what_will_my_donation_be_spent_on">
                                <p>
                                    You can find an overview of our financials <a href="https://drive.google.com/file/d/1syiUgEq-Or-GOfvBYY8k3fgTu5UZvLey/view" rel="noopener noreferrer" target="_blank">here</a> (apologies, it's in Dutch, since that's where
                                    our headquarter is based). In short, the main part of our budget goes to website and
                                    organizational costs (e.g. hosting, membership at a co-working space, banking costs). There are
                                    also costs related to events, but we generally break even on these events, e.g. by selling
                                    tickets, so they pay for themselves. As a team of volunteers, we manage to do a lot with just a
                                    little - your donation will go a long way. Our ambitious goal is to start the crowdacting
                                    movement and inspire millions of people to act for good by the end of 2020. All money is spent
                                    towards that goal.
                                </p>
                        </Faq>

                        <Faq title="Who started CollAction?" collapsed={true} faqId="who_started_collaction">
                            <p>
                                CollAction is started by a <a href="/about#team">team</a> of optimistic and pragmatic people that believe we can make this
                                world a better place through crowdacting. The concept of CollAction/crowdacting was born in The
                                Netherlands, but we now have an international team of around 20 volunteers.
                            </p>
                        </Faq>

                        <Faq title="How do I cancel my monthly donations" collapsed={true} faqId="cancel_monthly_donation">
                            <p>
                                You can cancel your recurring donations on your account page or by e-mailing
                                {" "}<a href="mailto:hello@collaction.org">hello@collaction.org</a>. Cancellations sent through e-mail will be processed within five days,
                                cancellations through your account page will be processed immediately.
                            </p>
                        </Faq>
                    </Grid>
                </Grid>
            </Section>
        </React.Fragment>
    );
};

export default DonationPage;