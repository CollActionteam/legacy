import {RouteComponentProps} from "react-router-dom";
import React from "react";
import {Grid, Hidden} from "@material-ui/core";
import {Section} from "../../../components/Section/Section";
import CrowdactionCard from "../../../components/CrowdactionCard/CrowdactionCard";
import {gql, useQuery} from "@apollo/client";
import {ICrowdaction} from "../../../api/types";
import {Fragments} from "../../../api/fragments";
import styles from "./ThankYouCommit.module.scss";
import CrowdactionShare from "../../../components/CrowdactionShare/CrowdactionShare";
import DonationLayout from "../../../components/DonationLayout/DonationLayout";

type TParams = {
    slug: string,
    crowdactionId: string
}

const ThankYouCommitPage = ({match}: RouteComponentProps<TParams>): any => {
    const {crowdactionId} = match.params;
    const {data} = useQuery(GET_CROWDACTION, {variables: {id: crowdactionId}});
    const crowdaction = (data?.crowdaction ?? null) as ICrowdaction | null;
    return <>
        <Section className={styles.header}>
            <Grid container spacing={10}>
                <Hidden smDown>
                    <Grid item md={5}/>
                </Hidden>
                <Grid item sm={12} md={7}>
                    <h2>Thank you for joining this crowdaction!</h2>
                </Grid>
            </Grid>
        </Section>
        <Section className={styles.section}>
            <Grid container spacing={10}>
                <Hidden smDown>
                    <Grid item md={5} className={styles.crowdactionGridItem}>
                        {crowdaction && <CrowdactionCard crowdaction={crowdaction}/>}
                    </Grid>
                </Hidden>
                <Grid item sm={12} md={7}>
                    <h3>Now go make waves and share it!</h3>

                    {crowdaction && <CrowdactionShare crowdaction={crowdaction} />}
                    <br/>
                </Grid>
            </Grid>
        </Section>

        <DonationLayout />

    </>
}

const GET_CROWDACTION = gql`
  query GetCrowdaction($id: ID) {
    crowdaction(id: $id) {
      ${Fragments.crowdactionDetail}
    }
  }
`;


export default ThankYouCommitPage;