import React, { useEffect } from "react";
import { gql, useQuery } from "@apollo/client";
import { Fragments } from "../../api/fragments";
import { Grid } from "@material-ui/core";

import CrowdactionCard from "../CrowdactionCard/CrowdactionCard";
import { CrowdactionStatusFilter, ICrowdaction } from "../../api/types";

import Loader from "../Loader/Loader";
import { Alert } from "../Alert/Alert";

interface ICrowdactionListProps {
  category?: string;
  status?: string;
}

export default ({ category, status = CrowdactionStatusFilter.Open }: ICrowdactionListProps) => {
  const { data, loading, error } = useQuery(
    FIND_CROWDACTIONS,
    category
      ? {
          variables: {
            category: category,
            status: status,
          },
        }
      : {
          variables: {
            status: status,
          },
        }
  );

  useEffect(() => {
    if (error) {
      console.error(error.message);
    }
  }, [ error ]);

  if (loading) {
    return <Loader />;
  }

  return (
    <>
      <Alert type="error" text={error?.message} />
      <Grid container spacing={3}>
        {data?.crowdactions && data.crowdactions.length ? (
          data.crowdactions.map((crowdaction: ICrowdaction, index: number) => (
            <Grid item xs={12} sm={6} md={4} key={index}>
              <CrowdactionCard crowdaction={crowdaction} />
            </Grid>
          ))
        ) : (
          <div>No crowdactions here yet.</div>
        )}
      </Grid>
    </>
  );
};

const FIND_CROWDACTIONS = gql`
  query FindCrowdactions($category: Category, $status: SearchCrowdactionStatus) {
    crowdactions(category: $category, status: $status) {
      ${Fragments.crowdactionDetail}
    }
  }
`;
