import { gql, useQuery } from "@apollo/client";
import { useLocation, useHistory } from "react-router-dom";
import React from "react";
import { Alert } from "../../components/Alert/Alert";
import Loader from "../../components/Loader/Loader";

const DonationReturnPage = () => {
    const searchParams = new URLSearchParams(useLocation().search);
    const source = searchParams.get('source');
    const clientSecret = searchParams.get('client_secret');
    const history = useHistory();

    const { data, loading, error } = useQuery(
        IDEAL_PAYMENT_SUCCEEDED,
        {
            variables: {
                source: source,
                clientSecret: clientSecret
            }
        }
    );

    if (error) {
        return <Alert type="error" text="Unable to find iDeal information" />;
    } else if (!loading && data.donation.hasIDealPaymentSucceeded) {
        history.push('/donate/thankyou');
    } else if (!loading && data.donation.hasIDealPaymentSucceeded === false) {
        history.push('/donate');
    }
    return <Loader />;
};

export default DonationReturnPage;

const IDEAL_PAYMENT_SUCCEEDED = gql`
    query IDealPaymentSucceeded($source: String!, $clientSecret: String!) {
        donation {
            hasIDealPaymentSucceeded(source: $source, clientSecret: $clientSecret)
        }
    }
`;