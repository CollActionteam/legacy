import React from "react";
import { Helmet } from "react-helmet";
import DonationLayout from "../../components/DonationLayout/DonationLayout";

const DonationThankYouPage = () => <>
    <Helmet>
        <title>Thank you for supporting CollAction!</title>
        <meta name="description" content="Thank you for supporting CollAction!"/>
    </Helmet>
    <DonationLayout thankYouPage />
</>;

export default DonationThankYouPage;