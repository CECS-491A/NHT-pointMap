<template>
    <div>
        <nav>
            <ul>
                <li>
                    <router-link class="navElement" to="/">Hello World</router-link>
                </li>
                <li>
                    <router-link class="navElement" to="/dashboard">Dashboard</router-link>
                </li>
                <li>
                    <router-link class="navElement" to="/mapview">MapView</router-link>
                </li>
                <li>
                    <router-link class="navElement" to="/logout">Logout</router-link>
                </li>
            </ul>
        </nav>
    </div>
</template>

<script>
export default {
    data() {
    return {
    }
  }
}
</script>


<style scoped>
    ul {
        list-style-type: none;
        margin: 0;
        padding: 0;
        overflow: hidden;
        background-color: #333;
    }

    li {
        float: left;
    }

    li:last-child{
        border-right: none;
        float: right;
    }
    .navElement {
        display: block;
        color: white;
        text-align: center;
        padding: 14px 16px;
        text-decoration: none;
    }

    .navElement:hover {
        background-color: #111;
    }
    .navElement:visited{
        color: white;
    }

    

    .navElement:hover:not(.active){
        background-color: #111;
    }

    .router-link-exact-active{
        background-color: #4CAF50;
    }
</style>
