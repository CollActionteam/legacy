import React from "react";
import CrowdactionCard from "../../../components/CrowdactionCard/CrowdactionCard";
import { Fragments } from "../../../api/fragments";
import { useQuery, gql } from "@apollo/client";
import { RouteComponentProps } from "react-router-dom";
import { ICrowdaction } from "../../../api/types";
import Loader from "../../../components/Loader/Loader";
import { Alert } from "../../../components/Alert/Alert";
import styles from "./CrowdactionWidget.module.scss";
import { Helmet } from "react-helmet";

const GET_CROWDACTION = gql`
    query GetCrowdaction($id: ID!) {
        crowdaction(id: $id) {
            ${Fragments.crowdactionDetail}
        }
    }
`;

type TParams = {
  slug: string,
  crowdactionId: string
}

const CrowdactionWidgetPage = ({ match } : RouteComponentProps<TParams>): any => {
    const { data, loading, error } = useQuery(
        GET_CROWDACTION,
        {
            variables: {
                id: match.params.crowdactionId
            }
        }
    );
    const crowdaction: ICrowdaction | undefined = data?.crowdaction;
    return <div className={styles.crowdactionWidget}>
        <Helmet>
            <title>Crowdaction { crowdaction?.name ?? "Loading" } widget</title>
            <meta name="description" content={`Crowdaction ${crowdaction?.name ?? "Loading"} widget`} />
        </Helmet>
        <Alert type="error" text={error?.message} />
        { loading ? <Loader /> : null }
        { crowdaction ? <CrowdactionCard target="_blank" crowdaction={crowdaction} /> : null }
    </div>
}

export default CrowdactionWidgetPage;