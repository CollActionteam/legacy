import React from "react";
import { Helmet } from "react-helmet";
import DonationLayout from "../../components/DonationLayout/DonationLayout";

const DonationPage = () => <>
            <Helmet>
                <title>Donate To CollAction</title>
                <meta name="description" content="Donate To CollAction"/>
            </Helmet>
            <DonationLayout />
        </>;

export default DonationPage;