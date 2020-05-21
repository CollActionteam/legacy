import React, { useState, useEffect } from "react";
import { gql, useQuery, useMutation, DataProxy, FetchResult } from "@apollo/client";
import Loader from "../Loader/Loader";
import { Card, CardContent, Grid, CardActions, TextField } from "@material-ui/core";
import Formatter from "../../formatter";
import { Button } from "../Button/Button";
import { useUser } from '../../providers/UserProvider';
import { Alert } from "../Alert/Alert";

interface ICrowdactionCommentsProps {
    id: string;
}

const DELETE_COMMENT = gql`
  mutation($commentId: ID!) {
    crowdaction {
      deleteComment(commentId: $commentId)
    }
  }
`;

const CREATE_COMMENT = gql`
  mutation($crowdactionId: ID!, $comment: String!) {
    crowdaction {
      createComment(crowdactionId: $crowdactionId, comment: $comment) {
        id
        comment
        commentedAt
        user {
          id
          fullName
        }
      }
    }
  }
`;

const GET_COMMENTS = gql`
  query($crowdactionId: ID!, $cursor: String) {
    crowdaction(id: $crowdactionId) {
      id
      comments(after: $cursor, first: 10, orderBy: { path: "commentedAt", descending: true }) @connection(key: "comment", filter: ["crowdactionId"]) {
        edges {
          node {
            id
            comment
            commentedAt
            user {
              id
              fullName
            }
          }
        }
        pageInfo {
          endCursor 
          hasNextPage
        }
      }
    }
  }`;

export default ({ id }: ICrowdactionCommentsProps) => {
    const user = useUser();
    const [ cursors, setCursors ] = useState<string[]>([]);
    const [ comment, setComment ] = useState("");
    const { data, loading, fetchMore, error } = useQuery<any>(
      GET_COMMENTS,
      {
        variables: {
          crowdactionId: id
        }
      }
    );
    const [ deleteComment ] = useMutation(
      DELETE_COMMENT,
      {
        update: (cache: DataProxy, mutationResult: FetchResult) => {
          console.log('need to update cache');
        }
      });
    const [ createComment ] = useMutation(CREATE_COMMENT,
      {
        variables: {
          crowdactionId: id,
          comment: comment
        },
        update: (cache: DataProxy, mutationResult: FetchResult) => {
          const currentComments: any = cache.readQuery({
            query: GET_COMMENTS,
            variables: { crowdactionId: id }
          });
          if (mutationResult.data) {
            const newCommentEdge = { node: mutationResult.data.crowdaction.createComment };
            cache.writeQuery({
              query: GET_COMMENTS,
              variables: { crowdactionId: id },
              data: {
                crowdaction: {
                  id: id,
                  comments: {
                    edges: [ newCommentEdge, ...currentComments.crowdaction.comments.edges ],
                    pageInfo: currentComments.crowdaction.comments.pageInfo
                  }
                }
              }
            });
          }
        }
      });
    const loadMore = () => {
      const newCursor = data.crowdaction.comments.pageInfo.endCursor;
      setCursors([...cursors, newCursor]);
      fetchMore({
        variables: {
          cursor: newCursor
        },
        updateQuery: (previousResult, { fetchMoreResult }) => {
          const newCrowdaction = fetchMoreResult.crowdaction;
          const previousCrowdaction = previousResult.crowdaction;
          const newEdges = newCrowdaction.comments.edges;
          const pageInfo = newCrowdaction.comments.pageInfo;

          return newEdges.length
            ? {
              // Put the new comments at the end of the list and update `pageInfo`
              // so we have the new `endCursor` and `hasNextPage` values
              crowdaction: {
                id: previousCrowdaction.id,
                comments: {
                  __typename: previousCrowdaction.comments.__typename,
                  edges: [...previousCrowdaction.comments.edges, ...newEdges],
                  pageInfo
                }
              }
            }
          : previousResult;
        }
      })        
    }

    useEffect(() => {
      if (error) {
        console.error(error?.message);
      }
    }, [ error ]);

    return <>
        <Grid container spacing={4}>
          <Alert type="error" text={error?.message} />
          { loading ? <Grid item xs={12}><Loader /></Grid> : null }
          {
            user ?
                <Grid item xs={12}>
                  <Card>
                    <CardContent>
                      <TextField value={comment} onChange={(ev) => setComment(ev.target.value)} label="Comment about the crowdaction" multiline rows={5} fullWidth />
                    </CardContent>
                    <CardActions>
                      <Button onClick={() => { createComment(); setComment("") }}>Comment</Button>
                    </CardActions>
                  </Card>
                </Grid>
                : null
          }
          {
            data?.crowdaction?.comments?.edges?.map((edge: any) => {
                const comment = edge.node;
                const commentDate = new Date(comment.commentedAt);
                return <Grid key={comment.id} item xs={12}>
                  <Card>
                    <CardContent>
                      <h4>{ comment.user.fullName }</h4>
                      {comment.comment.split('\n').map((ic: string) => <p key={ic}>{ ic }</p>) }
                      <p><em>{ Formatter.date(commentDate) } { Formatter.time(commentDate) }</em></p>
                    </CardContent>
                    {
                     user?.isAdmin ?
                      <CardActions>
                        <Button onClick={() => deleteComment({ variables: { commentId: comment.id }})}>Delete Comment</Button>
                      </CardActions> :
                      null
                    }
                  </Card>
                </Grid>;
              })
          }
          { data?.crowdaction?.comments?.pageInfo?.hasNextPage ? 
              <Grid item xs={12}><Button onClick={() => loadMore()}>Load More</Button></Grid> : 
              null
          }
        </Grid>
    </>;
}